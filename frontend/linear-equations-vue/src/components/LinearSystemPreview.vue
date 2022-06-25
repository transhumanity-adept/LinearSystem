<template>
  <v-row class="ma-0">
    <v-col class="pa-0 mr-2" align="center">
      <v-simple-table class="matrix">
        <template v-slot:default>
          <tbody>
          <tr
              v-for="(row, index) in matrixA"
              :key="index"
          >
            <td
              v-for="(value, index) in row"
              :key="index"
              style="font-size: 18px !important;"
            >
              {{ value }}
            </td>
          </tr>
          </tbody>
        </template>
      </v-simple-table>
    </v-col>
    <v-col class="pa-0 ml-2 mr-2 d-flex align-center" style="font-size: 25px !important;">
      *
    </v-col>
    <v-col class="pa-0 ml-2 mr-2" align="center">
      <v-simple-table class="matrix">
        <template v-slot:default>
          <tbody>
          <tr
              v-for="index in vectorB.length"
              :key="index"
          >
            <td style="font-size: 16px !important;">x</td>
          </tr>
          </tbody>
        </template>
      </v-simple-table>
    </v-col>
    <v-col class="pa-0 ml-2 mr-2 d-flex align-center" align="center" style="font-size: 25px !important;">
      =
    </v-col>
    <v-col class="pa-0 ml-2" align="center">
      <v-simple-table class="matrix">
        <template v-slot:default>
          <tbody>
          <tr
              v-for="(vectorValue, index) in vectorB"
              :key="index"
          >
            <td
                style="font-size: 20px !important;"
            >
              {{ vectorValue }}
            </td>
          </tr>
          </tbody>
        </template>
      </v-simple-table>
    </v-col>
  </v-row>
</template>

<script>
export default {
  name: "LinearSystemPreview",
  props: {
    matrixA: Array,
    vectorB: Array,
  },
  data: function () {
    return {

    }
  },
}
</script>

<style scoped>
.matrix {
  border-left: 2px solid #ACACAC;
  border-right: 2px solid #ACACAC;
}
</style>