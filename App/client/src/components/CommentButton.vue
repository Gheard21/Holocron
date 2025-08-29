<script setup lang="ts">
import { reactive, ref, watch } from "vue";
import { useAuth0 } from "@auth0/auth0-vue";
import { storeToRefs } from "pinia";
import { usePeopleStore } from "@/stores/usePeopleStore";

type ReviewForm = {
  date: string; // ISO yyyy-mm-dd
  description: string;
  rating: number | null;
};

const { isAuthenticated, getAccessTokenSilently } = useAuth0();

// access the currently selected person from the store
const peopleStore = usePeopleStore();
const { selectedPerson } = storeToRefs(peopleStore);

const dialog = ref(false);
const submitting = ref(false);
const errorMessage = ref("");
const formRef = ref();
const commentsCount = ref<number | null>(null);
const loadingComments = ref(false);
// Removed per-user has-commented check since the endpoint is unavailable

const form = reactive<ReviewForm>({
  date: "",
  description: "",
  rating: null,
});

const ratingItems = Array.from({ length: 10 }, (_, i) => ({
  title: String(i + 1),
  value: i + 1,
}));

const rules = {
  required: (v: unknown) => !!v || v === 0 || "Required",
  minLen: (n: number) => (v: string) =>
    (v && v.length >= n) || `Min ${n} characters`,
};

function openDialog() {
  if (!isAuthenticated.value || !selectedPerson.value) return;
  errorMessage.value = "";
  dialog.value = true;
}

function resetForm() {
  form.date = "";
  form.description = "";
  form.rating = null;
  (formRef.value as any)?.resetValidation?.();
}

async function onSubmit() {
  if (submitting.value) return;
  errorMessage.value = "";
  const result = await (formRef.value as any)?.validate?.();
  if (result && result.valid === false) return;

  submitting.value = true;
  try {
    if (!selectedPerson.value) throw new Error("No person selected");

    // Build request matching API NewCommentRequest
    const payload = {
      Name: selectedPerson.value.name,
      DateWatched: form.date ? new Date(form.date).toISOString() : null,
      Rating: form.rating,
      Review: form.description,
    };

    const token = await getAccessTokenSilently();
    const response = await fetch("http://localhost:5275/api/comments", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      let msg = "";
      try {
        const data = await response.json();
        if (Array.isArray(data)) {
          msg = data.map((e: any) => e.errorMessage ?? e).join("\n");
        } else if ((data as any)?.title) {
          msg = (data as any).title as string;
        }
      } catch {
        msg = await response.text().catch(() => "");
      }
      throw new Error(msg || `HTTP ${response.status}`);
    }

    // refresh comment count on success
    await fetchCommentsCount();
    dialog.value = false;
    resetForm();
  } catch (err: any) {
    errorMessage.value = `Review submission failed: ${
      err?.message || "Unknown error"
    }`;
  } finally {
    submitting.value = false;
  }
}

// Fetch total comments for selected person
const fetchCommentsCount = async () => {
  const person = selectedPerson.value;
  if (!person) {
    commentsCount.value = null;
    return;
  }
  try {
    loadingComments.value = true;
    const name = encodeURIComponent(person.name);
    const resp = await fetch(
      `http://localhost:5275/api/comments/${name}/count`
    );
    if (resp.ok) {
      const data = await resp.json();
      const count = data && typeof data.count === "number" ? data.count : null;
      commentsCount.value = count;
    } else {
      commentsCount.value = null;
    }
  } catch {
    commentsCount.value = null;
  } finally {
    loadingComments.value = false;
  }
};

// Removed fetchHasCommented()

// Prefill name (kept in case you add a name field back later)
watch(dialog, (isOpen) => {});

// Refresh comments count when selection changes
watch(
  selectedPerson,
  () => {
    fetchCommentsCount();
  },
  { immediate: true }
);
</script>

<template>
  <div class="comment-wrapper d-flex flex-column align-center">
    <v-btn
      icon="mdi-comment"
      :class="
        !isAuthenticated || !selectedPerson
          ? 'disabled-btn'
          : 'starwars-yellow-btn'
      "
      :disabled="!isAuthenticated || !selectedPerson"
      :title="
        !isAuthenticated
          ? 'Sign in to comment'
          : !selectedPerson
          ? 'Select a person first'
          : 'Comment'
      "
      @click="openDialog"
    >
    </v-btn>
    <div v-if="selectedPerson" class="text-caption mt-1 comments-text">
      <span v-if="commentsCount !== null">
        {{ commentsCount }} {{ commentsCount === 1 ? "comment" : "comments" }}
      </span>
      <span v-else>
        {{ loadingComments ? "Loading…" : "—" }}
      </span>
    </div>
  </div>

  <v-dialog v-model="dialog" max-width="640">
    <v-card>
      <v-card-title class="text-h6">Write a Review</v-card-title>
      <v-card-text>
        <v-alert v-if="errorMessage" type="error" variant="tonal" class="mb-4">
          {{ errorMessage }}
        </v-alert>

        <v-form ref="formRef" @submit.prevent="onSubmit">
          <v-text-field
            v-model="form.date"
            label="Date Watched"
            type="date"
            :rules="[rules.required]"
          />

          <v-select
            v-model="form.rating"
            :items="ratingItems"
            label="Rating (1-10)"
            :rules="[rules.required]"
          />

          <v-textarea
            v-model="form.description"
            label="Review Details"
            :rows="4"
            auto-grow
            counter="1000"
            :rules="[rules.required, rules.minLen(10)]"
          />
        </v-form>
      </v-card-text>

      <v-card-actions>
        <v-btn variant="text" @click="dialog = false" :disabled="submitting"
          >Cancel</v-btn
        >
        <v-spacer />
        <v-btn color="primary" :loading="submitting" @click="onSubmit"
          >Submit</v-btn
        >
      </v-card-actions>
    </v-card>
  </v-dialog>
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

.comments-text {
  color: #888;
}
</style>
