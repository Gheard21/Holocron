<script setup>
import { useAuth0 } from '@auth0/auth0-vue'

const { loginWithRedirect, logout, isAuthenticated, user } = useAuth0()

const login = () => loginWithRedirect()
const signout = () =>
  logout({ logoutParams: { returnTo: window.location.origin } })
</script>

<template>

</template>
