<script setup lang="ts">
import type { Person } from '@/types/Person';
import LikeButton from './LikeButton.vue';

defineProps<{ person: Person }>();
</script>

<template>
  <v-card class="details-card-with-like">
    <div class="like-btn-wrapper">
      <LikeButton />
    </div>
    <v-card-title class="text-h5">{{ person.name }}</v-card-title>
    <v-card-text>
      <v-list dense>
        <v-list-item>
          <v-list-item-title>Height: {{ person.height }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title>Mass: {{ person.mass }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title>Hair Color: {{ person.hair_color }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title>Skin Color: {{ person.skin_color }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title>Eye Color: {{ person.eye_color }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title>Birth Year: {{ person.birth_year }}</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-list-item-title>Gender: {{ person.gender }}</v-list-item-title>
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
}
</style>
