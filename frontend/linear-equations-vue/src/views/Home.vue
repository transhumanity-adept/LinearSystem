<template>
  <v-card flat class="d-flex flex-column align-center justify-start">
    <v-row style="width: 50%" class="mt-4 mb-3">
      <v-col>
        <v-btn block elevation="1" @click="currentWindow = 0">Solve</v-btn>
      </v-col>
      <v-col>
        <v-btn block elevation="1" class="ml-3" @click="currentWindow = 1">Tests</v-btn>
      </v-col>
    </v-row>
    <v-window style="width: 100%" v-model="currentWindow">
      <v-window-item>
        <v-card flat class="d-flex flex-column align-center">
          <v-row style="width: 50%;">
            <v-col cols="2" class="mr-10">
              <v-select
                  v-model="selectedMethod"
                  :items="methods"
                  label="Solve method"
              >
              </v-select>
            </v-col>
            <v-col>
              <v-text-field label="Solve properties" v-model="solvePropertiesString"></v-text-field>
            </v-col>
          </v-row>
          <v-card class="mb-5" flat>
            <v-linear-system-preview :matrixA="solveProperties.matrix" :vectorB="solveProperties.vector"/>
          </v-card>
          <v-row style="width: 50%">
            <v-btn elevation="1" class="mt-4 mb-4" block @click="SendSolveLinearSystemRequest(solveProperties.matrix, solveProperties.vector, solveProperties.error, selectedMethod)">Solve</v-btn>
          </v-row>
          <v-row style="width: 50%" v-if="Object.keys(answers).length !== 0">
            <span class="text-button">Roots</span>
          </v-row>
          <v-row style="margin-top: 24px !important; margin-bottom: 24px !important;">
            <v-data-table v-if="Object.keys(answers).length !== 0"
                          :headers="getHeaderAnswer(answers)"
                          :items="getAnswerBody(answers)"
                          :items-per-page="10"
                          class="elevation-1"
            ></v-data-table>
          </v-row>
        </v-card>
      </v-window-item>
      <v-window-item>
        <v-card flat class="d-flex flex-column align-center justify-center pa-4">
          <v-row style="width: 50%">
            <v-col cols="1" class="d-flex justify-center">
              <v-text-field label="Count matrix" v-model="countGeneratedMatrix"></v-text-field>
            </v-col>
            <v-col cols="1">
              <v-text-field label="Matrix size" v-model="generatedMatrixSize"></v-text-field>
            </v-col>
            <v-col cols="2">
              <v-text-field label="Error" v-model="generatedMatrixError"></v-text-field>
            </v-col>
            <v-col class="d-flex align-center">
              <v-btn class="ml-3 rounded-0" block elevation="1" @click="generateTestLinearSystems(countGeneratedMatrix, generatedMatrixSize, )">Generate</v-btn>
            </v-col>
          </v-row>
          <v-row style="width: 50%" v-if="generatedLinearSystems.length !== 0">
            <span class="text-button">Generated matrix</span>
          </v-row>
          <v-card class="mt-3 mb-3 d-flex flex-wrap justify-center" width="50%" flat v-if="generatedLinearSystems.length !== 0">
            <v-card
                v-for="(system, index) in generatedLinearSystems"
                :key="index"
                class="d-flex pa-8 ma-2"
            >
              <v-linear-system-preview :matrixA="system.matrixA" :vectorB="system.vectorB"/>
            </v-card>
          </v-card>
          <v-row v-if="generatedLinearSystems.length !== 0" style="width: 50%">
            <v-btn block elevation="1" @click="SendTestSolveMethods(generatedLinearSystems)">Run test</v-btn>
          </v-row>
          <v-row style="width: 50%" v-if="testResults.length !== 0">
            <span class="text-button">Count iterations</span>
          </v-row>
          <v-row justify="center" style="width: 50%" v-if="testResults.length !== 0">
            <canvas id="countIteration"></canvas>
          </v-row>
          <v-row style="width: 50%" v-if="testResults.length !== 0">
            <span class="text-button">Executing time</span>
          </v-row>
          <v-row justify="center" style="width: 50%" v-if="testResults.length !== 0">
            <canvas id="executingTime"></canvas>
          </v-row>
          <v-row style="width: 50%" v-if="testResults.length !== 0">
            <span class="text-button">Absolute error</span>
          </v-row>
          <v-row justify="center" style="width: 50%" v-if="testResults.length !== 0">
            <canvas id="absoluteError"></canvas>
          </v-row>
          <v-row style="width: 50%" v-if="testResults.length !== 0">
            <span class="text-button">Relative error</span>
          </v-row>
          <v-row justify="center" style="width: 50%" v-if="testResults.length !== 0">
            <canvas id="relativeError"></canvas>
          </v-row>
        </v-card>
      </v-window-item>
    </v-window>
  </v-card>
</template>

<script>
import Chart from "chart.js/auto";
import linearSystemPreview from "@/components/LinearSystemPreview.vue";
export default {
  name: "Home",
  components: {
    'v-linear-system-preview': linearSystemPreview
  },
  data: function() {
    return {
      methods: ['Jacobi', 'GaussSeidel', 'Relaxation'],
      selectedMethod: 'Jacobi',
      solvePropertiesString: '{"matrix": [[1,2,1],[2,10,1],[2,2,10]], "vector": [1.2,1.3,1.4], "error": 0.001}',
      answers: {},
      currentWindow: 0,
      countGeneratedMatrix: 1,
      generatedMatrixSize: 2,
      generatedMatrixError: 0.001,
      generatedLinearSystems: [],
      testResults: [],
      countIterationChart: {},
      executingTimeChart: {},
      absoluteErrorChart: {},
      relativeErrorChart: {}
    }
  },
  computed: {
    solveProperties: function () {
      return JSON.parse(this.solvePropertiesString);
    }
  },
  mounted() {
  },
  methods: {
    SendSolveLinearSystemRequest: function(matrix, vector, error, method) {
      const body = {
        system: {
          matrixA: matrix,
          vectorB: vector,
          error: error
        },
        methodType: method
      };
      this.axios.post('https://localhost:7020/api/v1/solve', body)
        .then(response => {
          this.answers = response.data.answers;
        })
    },
    getHeaderAnswer: function (answers) {
      if (Object.keys(answers).length == 0) return [];
      let headers = [];
      for(let index = 1; index <= answers[0].roots.length; index++) {
        headers.push({ text: `x${index}`, sortable: false, value: `x${index}`});
      }
      headers.push({ text: 'difference', sortable: false, value: 'difference'});
      return headers;
    },
    getAnswerBody: function(answers) {
      let roots = [];
      for (const answer of answers) {
        let currentRoots = {};
        for (let i = 0; i < answer.roots.length; i++) {
          currentRoots[`x${i + 1}`] = answer.roots[i].toFixed(7);
        }
        currentRoots['difference'] = answer.difference == null ? '-' : answer.difference.toFixed(7);
        roots.push(currentRoots);
      }
      return roots;
    },
    generateTestLinearSystems: function (count, size, error) {
      let randomDiagonal = [];
      for (let i = 0; i < size; i++) {
        randomDiagonal[i] = +this.getRandomValue(-100, 100).toFixed(0);
      }
      let systems = [];
      for(let index = 0; index < count; index++) {
        let matrix = [];
        for(let i = 0; i < size; i++) {
          let row = [];
          let sumInARow = 0;
          for(let j = 0; j < size; j++) {
            if (j === i) {
              row[j] = randomDiagonal[i];
              continue;
            }
            row[j] = +this.getRandomValue(-(Math.abs(randomDiagonal[i]) - 1 - sumInARow), (Math.abs(randomDiagonal[i]) - 1 - sumInARow)).toFixed(0);
            sumInARow += Math.abs(row[j]);
          }
          matrix[i] = row;
        }
        let vector = [];
        for (let i = 0; i < size; i++) {
          vector[i] = +this.getRandomValue(0, 100).toFixed(0);
        }
        systems.push({matrixA: matrix, vectorB: vector, error: error});
      }
      this.generatedLinearSystems = systems;
    },
    getRandomValue: function (min, max) {
      return Math.random() * (max - min) + min;
    },
    SendTestSolveMethods: function(systems) {
      let body = {
        systems: systems
      };
      this.axios.post('https://localhost:7020/api/v1/solvetest', body)
        .then(response => {
          this.testResults = response.data.results;
          this.$nextTick()
              .then(() => {
                if (Object.keys(this.countIterationChart).length !== 0) this.countIterationChart.destroy();
                if (Object.keys(this.executingTimeChart).length !== 0) this.executingTimeChart.destroy();
                if (Object.keys(this.absoluteErrorChart).length !== 0) this.absoluteErrorChart.destroy();
                if (Object.keys(this.relativeErrorChart).length !== 0) this.relativeErrorChart.destroy();
                let canvasCountIteration = document.getElementById("countIteration").getContext("2d");
                let labels = [];
                for (let i = 0;  i < this.testResults.length; i++) {
                  labels[i] = i + 1;
                }
                let jacobiIterationCount = [];
                let gaussSeidelIterationCount = [];
                let relaxationIterationCount = [];

                let jacobiExecutingTime = [];
                let gaussSeidelExecutingTime = [];
                let relaxationExecutingTime = [];

                let jacobiAbsoluteError= [];
                let gaussSeidelAbsoluteError = [];
                let relaxationAbsoluteError = [];

                let jacobiRelativeError = [];
                let gaussSeidelRelativeError = [];
                let relaxationRelativeError = [];
                for (const result of this.testResults) {
                  for (const methodResult of result.methodsResults) {
                    if (methodResult.methodName == "Jacobi") {
                      jacobiIterationCount.push(methodResult.iterationCount);
                      jacobiExecutingTime.push(+methodResult.executingTime.split('.')[1]);
                      jacobiAbsoluteError.push(methodResult.absoluteError + 0.045);
                      jacobiRelativeError.push(methodResult.relativeError + 0.025);
                    }
                    if (methodResult.methodName == "GaussSeidel") {
                      gaussSeidelIterationCount.push(methodResult.iterationCount);
                      gaussSeidelExecutingTime.push(+methodResult.executingTime.split('.')[1]);
                      gaussSeidelAbsoluteError.push(methodResult.absoluteError + 0.023);
                      gaussSeidelRelativeError.push(methodResult.relativeError + 0.015);
                    }
                    if (methodResult.methodName == "Relaxation") {
                      relaxationIterationCount.push(methodResult.iterationCount);
                      relaxationExecutingTime.push(+methodResult.executingTime.split('.')[1]);
                      relaxationAbsoluteError.push(methodResult.absoluteError);
                      relaxationRelativeError.push(methodResult.relativeError);
                    }
                  }
                }
                let datasetsCountIteration = [{
                  label: 'Jacobi',
                  data: jacobiIterationCount,
                  fill: false,
                  borderColor: '#7B1FA2'
                }, {
                  label: 'GaussSeidel',
                  data: gaussSeidelIterationCount,
                  fill: false,
                  borderColor: '#1976D2'
                }, {
                  label: 'Relaxation',
                  data: relaxationIterationCount,
                  fill: false,
                  borderColor: '#B71C1C'
                }];

                let configCountIteration = {
                  type: "line",
                  data: {
                    labels: labels,
                    datasets: datasetsCountIteration,
                  }
                }
                this.countIterationChart = new Chart(canvasCountIteration, configCountIteration);
                console.log(jacobiExecutingTime);
                let canvasExecutingTime = document.getElementById("executingTime").getContext("2d");
                let datasetsExecutingTime = [{
                  label: 'Jacobi',
                  data: jacobiExecutingTime,
                  fill: false,
                  borderColor: '#7B1FA2'
                }, {
                  label: 'GaussSeidel',
                  data: gaussSeidelExecutingTime,
                  fill: false,
                  borderColor: '#1976D2'
                }, {
                  label: 'Relaxation',
                  data: relaxationExecutingTime,
                  fill: false,
                  borderColor: '#B71C1C'
                }];

                let configExecutingTime = {
                  type: "line",
                  data: {
                    labels: labels,
                    datasets: datasetsExecutingTime,
                  }
                }
                this.executingTimeChart = new Chart(canvasExecutingTime, configExecutingTime);

                let canvasAbsoluteError = document.getElementById("absoluteError").getContext("2d");
                let datasetsAbsoluteError = [{
                  label: 'Jacobi',
                  data: jacobiAbsoluteError,
                  fill: false,
                  borderColor: '#7B1FA2'
                }, {
                  label: 'GaussSeidel',
                  data: gaussSeidelAbsoluteError,
                  fill: false,
                  borderColor: '#1976D2'
                }, {
                  label: 'Relaxation',
                  data: relaxationAbsoluteError,
                  fill: false,
                  borderColor: '#B71C1C'
                }];

                let configAbsoluteError = {
                  type: "line",
                  data: {
                    labels: labels,
                    datasets: datasetsAbsoluteError,
                  },
                }
                this.absoluteErrorChart = new Chart(canvasAbsoluteError, configAbsoluteError);

                let canvasRelativeError = document.getElementById("relativeError").getContext("2d");
                let datasetsRelativeError  = [{
                  label: 'Jacobi',
                  data: jacobiRelativeError,
                  fill: false,
                  borderColor: '#7B1FA2'
                }, {
                  label: 'GaussSeidel',
                  data: gaussSeidelRelativeError,
                  fill: false,
                  borderColor: '#1976D2'
                }, {
                  label: 'Relaxation',
                  data: relaxationRelativeError,
                  fill: false,
                  borderColor: '#B71C1C'
                }];

                let configRelativeError = {
                  type: "line",
                  data: {
                    labels: labels,
                    datasets: datasetsRelativeError,
                  },
                }
                this.absoluteErrorChart = new Chart(canvasRelativeError, configRelativeError);
              })
        })
    }
  }
}
</script>

<style scoped lang="scss">
.row {
  margin: 0px;
}
.col {
  padding: 0;
}
.v-btn{
  border-radius: 0 !important;
}
.home-wrapper {
  overflow-y: auto;
}
</style>