<script setup lang="ts">
import Search from './Search.vue';
import Details from './Details.vue';
import Header from './Header.vue';
import { ref } from 'vue';
import type { Person } from '@/types/Person';

// Create a ref to store the selected person
const selectedPerson = ref<Person | null>(null);

// Handler function to receive the selected person from Search
const handlePersonSelected = (person : Person) => {
  selectedPerson.value = person;
};
</script>

<template>
  <v-container>
    <Header />
    <!-- Center the search at the top of the page, with same width as details -->
    <v-row justify="center">
      <v-col cols="12" sm="10" md="8" lg="6">
        <Search @person-selected="handlePersonSelected"/>
      </v-col>
    </v-row>

    <!-- Details section below search -->
    <v-row v-if="selectedPerson" justify="center">
      <v-col cols="12" sm="10" md="8" lg="6">
        <Details :person="selectedPerson" />
      </v-col>
    </v-row>
  </v-container>
</template>
