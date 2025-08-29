<script setup lang="ts">
import { useAuth0 } from '@auth0/auth0-vue';

const { isAuthenticated } = useAuth0();
</script>

<template>
  <v-btn
    icon="mdi-comment"
    :class="isAuthenticated ? 'starwars-yellow-btn' : 'disabled-btn'"
    :disabled="!isAuthenticated"
    :title="isAuthenticated ? 'Comment' : 'Sign in to comment'"
  >
  </v-btn>
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
</style>
