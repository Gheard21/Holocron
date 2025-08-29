// Utilities
import type { Person } from "@/types/Person";
import { defineStore } from "pinia";

export const usePeopleStore = defineStore("people", {
  state: () => ({
    people: [] as Person[],
    loading: false,
    error: null as string | null,
    selectedPerson: null as Person | null,
  }),
  actions: {
    async fetchPeople(url = "https://swapi.info/api/people") {
      this.loading = true;
      this.error = null;

      try {
        const response = await fetch(url);

        if (!response.ok) {
          throw new Error(`Failed to fetch people: ${response.status}`);
        }

        const data = (await response.json()) as Person[];

        this.people = data;
      } catch (error) {
        this.error =
          error instanceof Error ? error.message : "Unknown error occurred";
        console.error("Error fetching people:", error);
      } finally {
        this.loading = false;
      }
    },
    async getPerson(id: string) {
      this.loading = true;
      this.error = null;

      try {
        const response = await fetch(`https://swapi.info/api/people/${id}/`);

        if (!response.ok) {
          throw new Error(`Failed to fetch person: ${response.status}`);
        }

        const person = (await response.json()) as Person;

        // Update or add the person in the people array
        const index = this.people.findIndex((p) => p.url === person.url);
        if (index >= 0) {
          this.people[index] = person;
        } else {
          this.people.push(person);
        }

        return person;
      } catch (error) {
        this.error =
          error instanceof Error ? error.message : "Unknown error occurred";
        console.error("Error fetching person:", error);
        return null;
      } finally {
        this.loading = false;
      }
    },
    setSelectedPerson(person: Person | null) {
      this.selectedPerson = person;
    },
    clearSelectedPerson() {
      this.selectedPerson = null;
    },
  },
});
