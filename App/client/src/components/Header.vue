<script setup>
import { ref } from 'vue'

const isLoggedIn = ref(false)

function onAuthClick() {
  isLoggedIn.value = !isLoggedIn.value
}
</script>

<template>
  <v-app-bar color="black" dark>
    <v-toolbar-title class="star-wars-yellow">
      Holocron
    </v-toolbar-title>
    <v-spacer />
    <v-btn
      class="star-wars-yellow"
      text
      @click="onAuthClick"
    >
      {{ isLoggedIn ? 'Logout' : 'Login' }}
    </v-btn>
  </v-app-bar>
</template>

<style scoped>
.star-wars-yellow {
  color: #ffe81f !important;
  font-family: 'Star Jedi', Arial, sans-serif;
  letter-spacing: 2px;
}
</style>
