<script setup lang="ts">
import { usePeopleStore } from '@/stores/usePeopleStore';
import type { Person } from '@/types/Person';
import { computed, onMounted, ref } from 'vue';

const peopleStore = usePeopleStore();
const selectedItem = ref(null);

const emit = defineEmits(['person-selected']);

// Load people data when component mounts
onMounted(() => {
  if (peopleStore.people.length === 0) {
    peopleStore.fetchPeople();
  }
});

// Define a type for the autocomplete item structure
type PersonItem = {
  title: string;
  value: string;
  person: Person;
};

// Updated handleSelectionChange function with correct typing
const handleSelectionChange = (item: PersonItem | null) => {
  if (item) {
    // Emit the person object, not the entire autocomplete item
    emit('person-selected', item.person);
  }
};

// Format people data for autocomplete
const peopleItems = computed(() => {
  return peopleStore.people.map(person => ({
    title: person.name,
    value: person.url,
    person: person
  }));
});
</script>

<template>
  <v-autocomplete
    v-model="selectedItem"
    label="Search Characters"
    :items="peopleItems"
    :loading="peopleStore.loading"
    item-title="title"
    item-value="value"
    placeholder="Type to search Star Wars characters"
    return-object
    :no-data-text="peopleStore.error || 'No characters found'"
    @update:model-value="handleSelectionChange">
  </v-autocomplete>
</template>
