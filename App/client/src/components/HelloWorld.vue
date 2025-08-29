<script setup lang="ts">
import Search from "./Search.vue";
import Details from "./Details.vue";
import type { Person } from "@/types/Person";
import { useAuth0 } from "@auth0/auth0-vue";
import { storeToRefs } from "pinia";
import { usePeopleStore } from "@/stores/usePeopleStore";

const { loginWithRedirect, logout, isAuthenticated, user } = useAuth0();

const login = () => loginWithRedirect();
const signout = () =>
  logout({ logoutParams: { returnTo: window.location.origin } });

// Use store state for selected person so it's accessible app-wide
const peopleStore = usePeopleStore();
const { selectedPerson } = storeToRefs(peopleStore);

// Handler function to receive the selected person from Search
const handlePersonSelected = (person: Person) => {
  peopleStore.setSelectedPerson(person);
};
</script>

<template>
  <v-container>
    <v-app-bar>
      <v-spacer></v-spacer>
      <div v-if="isAuthenticated">
        <span>{{ user?.name }}</span>
        <v-btn @click="signout">Logout</v-btn>
      </div>
      <div v-else>
        <v-btn @click="login">Login</v-btn>
      </div>
    </v-app-bar>
    <!-- Center the search at the top of the page, with same width as details -->
    <v-row justify="center">
      <v-col cols="12" sm="10" md="8" lg="6">
        <Search @person-selected="handlePersonSelected" />
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
