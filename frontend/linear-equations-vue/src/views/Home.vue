<template>
  <v-icon>mdi-plus</v-icon>
</template>

<script>
export default {
  name: "Home",
  data: function() {
    return {

    }
  },
  mounted() {
    this.axios.get('https://jsonplaceholder.typicode.com/todos')
    .then(response => console.log(response));
  }
}
</script>

<style scoped lang="scss">

</style>