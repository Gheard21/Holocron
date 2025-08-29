<script setup lang="ts">
import type { Person } from '@/types/Person';
import LikeButton from './LikeButton.vue';
import CommentButton from './CommentButton.vue';

defineProps<{ person: Person }>();
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
          <v-list-item-title><v-icon icon="mdi-human-male-height"></v-icon> Height: {{ person.height }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title><v-icon icon="mdi-weight"></v-icon> Mass: {{ person.mass }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title><v-icon icon="mdi-hair-dryer"></v-icon> Hair Color: {{ person.hair_color }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title><v-icon icon="mdi-palette"></v-icon> Skin Color: {{ person.skin_color }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title><v-icon icon="mdi-eye"></v-icon> Eye Color: {{ person.eye_color }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title><v-icon icon="mdi-calendar-account-outline"></v-icon> Birth Year: {{ person.birth_year }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title><v-icon icon="mdi-gender-male-female"></v-icon> Gender: {{ person.gender }}</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-card-text>
  </v-card>
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
