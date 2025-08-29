<script setup lang="ts">
import { ref, watch } from "vue";
import { useAuth0 } from "@auth0/auth0-vue";
import { storeToRefs } from "pinia";
import { usePeopleStore } from "@/stores/usePeopleStore";

const { isAuthenticated, getAccessTokenSilently } = useAuth0();

// Access the currently selected person from the store
const peopleStore = usePeopleStore();
const { selectedPerson } = storeToRefs(peopleStore);

// Likes count state
const likesCount = ref<number | null>(null);
const loadingLikes = ref(false);
const userHasLiked = ref<boolean | null>(null);
const loadingHasLiked = ref(false);

const fetchLikes = async () => {
  const person = selectedPerson.value;
  if (!person) {
    likesCount.value = null;
    return;
  }
  try {
    loadingLikes.value = true;
    const name = encodeURIComponent(person.name);
    const resp = await fetch(`http://localhost:5275/api/likes/${name}`);
    if (resp.ok) {
      const num = await resp.json();
      likesCount.value = typeof num === "number" ? num : Number(num);
    } else {
      likesCount.value = null;
      console.error("Failed to fetch likes:", await resp.text());
    }
  } catch (e) {
    likesCount.value = null;
    console.error("Error fetching likes:", e);
  } finally {
    loadingLikes.value = false;
  }
};

const fetchHasLiked = async () => {
  // Only check when authenticated and person selected
  if (!isAuthenticated.value) {
    userHasLiked.value = null;
    return;
  }
  const person = selectedPerson.value;
  if (!person) {
    userHasLiked.value = null;
    return;
  }
  try {
    loadingHasLiked.value = true;
    const token = await getAccessTokenSilently();
    const name = encodeURIComponent(person.name);
    const resp = await fetch(`http://localhost:5275/api/likes/${name}/me`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    if (resp.ok) {
      const val = await resp.json();
      userHasLiked.value = Boolean(val);
    } else {
      userHasLiked.value = null;
      console.error("Failed to check hasLiked:", await resp.text());
    }
  } catch (e) {
    userHasLiked.value = null;
    console.error("Error checking hasLiked:", e);
  } finally {
    loadingHasLiked.value = false;
  }
};

// Refresh likes when selection changes
watch(
  selectedPerson,
  () => {
    fetchLikes();
    fetchHasLiked();
  },
  { immediate: true }
);

// Also re-check like state when auth state changes
watch(
  () => isAuthenticated.value,
  () => {
    fetchHasLiked();
  }
);

const handleClick = async () => {
  // Prevent calls when not authenticated or no selection
  if (!isAuthenticated.value || !selectedPerson?.value) return;

  try {
    const token = await getAccessTokenSilently();
    const name = encodeURIComponent(selectedPerson.value.name);

    const method = userHasLiked.value ? "DELETE" : "POST";
    const response = await fetch(`http://localhost:5275/api/likes/${name}`, {
      method,
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const msg = await response.text();
      console.error("Like toggle failed:", msg);
    }

    // Refresh both like count and user has-liked state
    await Promise.all([fetchLikes(), fetchHasLiked()]);
  } catch (err) {
    console.error("Error toggling like:", err);
  }
};
</script>

<template>
  <div class="like-wrapper d-flex flex-column align-center">
    <v-btn
      @click="handleClick"
      icon="mdi-thumb-up"
      :class="
        !isAuthenticated || !selectedPerson
          ? 'disabled-btn'
          : userHasLiked
          ? 'liked-blue-btn'
          : 'starwars-yellow-btn'
      "
      :disabled="!isAuthenticated || !selectedPerson"
      :title="
        !isAuthenticated
          ? 'Sign in to like'
          : !selectedPerson
          ? 'Select a person first'
          : userHasLiked
          ? 'Unlike'
          : 'Like'
      "
    >
    </v-btn>
    <div v-if="selectedPerson" class="text-caption mt-1 likes-text">
      <span v-if="likesCount !== null">
        {{ likesCount }} {{ likesCount === 1 ? "like" : "likes" }}
      </span>
      <span v-else>
        {{ loadingLikes ? "Loading…" : "—" }}
      </span>
    </div>
  </div>
</template>

<style scoped>
.starwars-yellow-btn {
  background-color: #ffe81f !important;
  color: #222;
}

.disabled-btn {
  background-color: #cccccc !important;
  color: #666666;
}

.likes-text {
  color: #888;
}

.liked-blue-btn {
  background-color: #1976d2 !important; /* Material primary blue */
  color: #fff;
}
</style>
