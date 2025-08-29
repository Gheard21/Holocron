<script setup lang="ts">
import type { Person } from "@/types/Person";
import LikeButton from "./LikeButton.vue";
import CommentButton from "./CommentButton.vue";
import { ref, watch } from "vue";

const props = defineProps<{ person: Person }>();

// Comments viewing dialog
const showComments = ref(false);
const loadingComments = ref(false);
const commentsError = ref("");
const comments = ref<
  Array<{ dateWatched: string; rating: number; review: string }>
>([]);

const fetchComments = async () => {
  commentsError.value = "";
  comments.value = [];
  if (!props.person) return;
  try {
    loadingComments.value = true;
    const name = encodeURIComponent(props.person.name);
    const resp = await fetch(`http://localhost:5275/api/comments/${name}`);
    if (!resp.ok) {
      commentsError.value = await resp.text();
      return;
    }
    const arr = await resp.json();
    comments.value = Array.isArray(arr)
      ? arr.map((c: any) => ({
          dateWatched: c.dateWatched,
          rating: c.rating,
          review: c.review,
        }))
      : [];
  } catch (e: any) {
    commentsError.value = e?.message || "Failed to load comments";
  } finally {
    loadingComments.value = false;
  }
};

watch(
  () => props.person?.name,
  () => {
    if (showComments.value) fetchComments();
  }
);
</script>

<template>
  <v-card class="details-card-with-like">
    <div class="like-btn-wrapper">
      <CommentButton class="mr-2" />
      <LikeButton />
    </div>
    <v-card-title class="text-h5">{{ person.name }}</v-card-title>
    <v-card-text>
      <v-list dense>
        <v-list-item>
          <v-list-item-title
            ><v-icon icon="mdi-human-male-height"></v-icon> Height:
            {{ person.height }}</v-list-item-title
          >
        </v-list-item>
        <v-list-item>
          <v-list-item-title
            ><v-icon icon="mdi-weight"></v-icon> Mass:
            {{ person.mass }}</v-list-item-title
          >
        </v-list-item>
        <v-list-item>
          <v-list-item-title
            ><v-icon icon="mdi-hair-dryer"></v-icon> Hair Color:
            {{ person.hair_color }}</v-list-item-title
          >
        </v-list-item>
        <v-list-item>
          <v-list-item-title
            ><v-icon icon="mdi-palette"></v-icon> Skin Color:
            {{ person.skin_color }}</v-list-item-title
          >
        </v-list-item>
        <v-list-item>
          <v-list-item-title
            ><v-icon icon="mdi-eye"></v-icon> Eye Color:
            {{ person.eye_color }}</v-list-item-title
          >
        </v-list-item>
        <v-list-item>
          <v-list-item-title
            ><v-icon icon="mdi-calendar-account-outline"></v-icon> Birth Year:
            {{ person.birth_year }}</v-list-item-title
          >
        </v-list-item>
        <v-list-item>
          <v-list-item-title
            ><v-icon icon="mdi-gender-male-female"></v-icon> Gender:
            {{ person.gender }}</v-list-item-title
          >
        </v-list-item>
      </v-list>
    </v-card-text>
    <v-card-actions class="justify-end pr-3 pb-2">
      <v-btn
        size="small"
        variant="tonal"
        prepend-icon="mdi-comment-text-outline"
        @click="
          showComments = true;
          fetchComments();
        "
        :title="`View comments for ${person.name}`"
      >
        View comments
      </v-btn>
    </v-card-actions>
  </v-card>
  <v-dialog v-model="showComments" max-width="720">
    <v-card>
      <v-card-title class="text-h6"
        >Comments for {{ props.person.name }}</v-card-title
      >
      <v-card-text>
        <v-alert v-if="commentsError" type="error" variant="tonal" class="mb-4">
          {{ commentsError }}
        </v-alert>
        <div v-if="loadingComments">Loading…</div>
        <div v-else>
          <div v-if="comments.length === 0" class="text-body-2">
            No comments yet.
          </div>
          <v-list v-else density="comfortable">
            <v-list-item v-for="(c, idx) in comments" :key="idx">
              <v-list-item-title>
                <strong>Rating:</strong> {{ c.rating }} / 10
              </v-list-item-title>
              <v-list-item-subtitle>
                <strong>Watched:</strong>
                {{ new Date(c.dateWatched).toLocaleDateString() }}
              </v-list-item-subtitle>
              <div class="mt-1">
                {{ c.review }}
              </div>
            </v-list-item>
          </v-list>
        </div>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn variant="text" @click="showComments = false">Close</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<style scoped>
.details-card-with-like {
  position: relative;
}
.like-btn-wrapper {
  position: absolute;
  top: 12px;
  right: 12px;
  z-index: 1;
  display: flex;
  align-items: center;
}
</style>
