<template>
  <div id="suggested-supplies">
    <h1>Supply Suggestions</h1>
    <ul class="supply_container">
      <div
        class="suggested_supplies"
        v-for="supply in sortedSupplies"
        :key="supply.supplyId"
      >
        <div class="supply_description">
          <h2>{{ supply.supplyName }}</h2>
          <h3>$ {{(supply.supplyCost)}}.00</h3>
          <button v-on:click="addSupplyToShoppingList(supply)">
            Add Item to shopping list
          </button>
        </div>
        <div class="image_container">
          <a href="https://www.ruralking.com">
            <img :src="supply.imageUrl" class="supply-image" />
          </a>
        </div>
      </div>
    </ul>
  </div>
</template>

<script>
import suggestService from '../../services/SuggestService'

export default {
<<<<<<< HEAD
  name: 'suggested-supplies',
  data() {
    return {
      suggestedSupplies: [],
    }
  },
  methods: {
    addSupplyToShoppingList(supply) {
      suggestService
        .addSupplyToShoppingList(supply)
        .then((response) => {
          if (response.status === 201) {
            this.$router.push('/Shop')
=======
        name: "suggested-supplies",
        data() {
          return {
            suggestedSupplies:[],
          }
        },
        methods:{
        addSupplyToShoppingList(supply){ 
          suggestService
              .addSupplyToShoppingList(supply)
              .then(response => {
                  if (response.status === 201) {
                  this.$router.push("/ShoppingList");
                  }
              })
              .catch(error => {
                  console.error(error);
              });                

>>>>>>> 698c994e53b2b9e69b545fce102db6b97cd60f04
          }
        })
        .catch((error) => {
          console.error(error)
        })
    },
  },
  created() {
    suggestService.getSupplyStore().then((response) => {
      this.$store.commit('SET_SUPPLIES', response.data)
    })
  },
  computed: {
    //will sort supplies to display to user based on plot dimensions(mulch) and supplies NOT already on the shopping list
    sortedSupplies() {
      return this.$store.state.supplies.filter((supply) => {
        return supply.supplyCost <= 10
      })
    },
    suppliesByTotalPlotSize() {
      return null
    },
    suppliesNotOnList() {
      return null
    },
  },
}
</script>

<style scoped>
.suggested-supplies {
  display: flex;
  flex-direction: column;
}
#suggested-supplies > h1 {
  font-size: 20px;
  margin: 50px auto;
  text-align: center;
  font-size: 28px;
  color: #fff;
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI';
  text-shadow: 0px 0px 10px #444;
}
.suggested_supplies {
  width: 1080px;
  
  justify-content: center;
  flex-basis: 50%;
  display: flex;
  background-color: rgba(168, 211, 189, 0.733);
  margin: 10px auto;
  border-radius: 10px;
  text-align: center;
}
.suggested_supplies li {
  margin: 0 auto;
}
.supply_description {
  margin: 10px;
}

img {
  height: 150px;
  width: 180px;
  margin: 10px;
  margin-right: 10px;
  border-radius: 10px;
}

button {
  box-shadow: 0 8px 16px 0 rgba(0, 0, 0, 0.2);
  border: none;
  height: 30px;
  font-weight: bold;
}
</style>
