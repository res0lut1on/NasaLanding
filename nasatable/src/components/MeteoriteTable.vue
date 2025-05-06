<template>
  <div @scroll="handleScroll" class="scroll-container">
    <div class="filters">
      <h2>Filters</h2>
      <div class="filter-group">
        <label>
          Year From:
          <select v-model="filters.yearFrom">
            <option value="">All</option>
            <option v-for="year in yearOptions" :key="year" :value="year">{{ year }}</option>
          </select>
        </label>
        <label>
          Year To:
          <select v-model="filters.yearTo">
            <option value="">All</option>
            <option v-for="year in yearOptions" :key="year" :value="year">{{ year }}</option>
          </select>
        </label>
        <label>
          Class:
          <select v-model="filters.meteoriteClass">
            <option value="">All</option>
            <option v-for="cls in classOptions" :key="cls" :value="cls">{{ cls }}</option>
          </select>
        </label>
        <label>
          Search:
          <input v-model="filters.search" placeholder="Meteorite name" />
        </label>
        <label>
          Sort By:
          <select v-model="filters.sortBy">
            <option value="name">Name</option>
            <option value="year">Year</option>
            <option value="mass">Mass</option>
          </select>
        </label>
        <label>
          Descending:
          <input type="checkbox" v-model="filters.sortDescending" />
        </label>
        <label>
          Group By Year:
          <input
            type="checkbox"
            v-model="filters.groupByYear"
            @change="resetAndFetchData"
          />
        </label>
      </div>
      <div class="filter-buttons">
        <button class="btn btn-primary" @click="resetAndFetchData">Apply</button>
        <button class="btn btn-secondary" @click="clearFilters">Clear</button>
      </div>
    </div>

    <table v-if="!filters.groupByYear" class="data-table">
      <thead>
        <tr>
          <th>Name</th>
          <th>Class</th>
          <th>Year</th>
          <th>Mass</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="m in meteoriteItems" :key="m.id">
          <td>{{ m.name }}</td>
          <td>{{ m.class }}</td>
          <td>{{ m.year }}</td>
          <td>{{ m.mass }}</td>
        </tr>
      </tbody>
    </table>

    <div v-else>
      <table class="data-table">
        <thead>
          <tr>
            <th>Year</th>
            <th>Total Count</th>
            <th>Total Mass</th>
            <th>Meteorites</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="group in groupedItems" :key="group.year">
            <td>{{ group.year }}</td>
            <td>{{ group.totalCount }}</td>
            <td>{{ group.totalMass }}</td>
            <td>
              <ul>
                <li v-for="m in group.meteorites" :key="m.id">
                  {{ m.name }} ({{ m.class }}, {{ m.mass }} г)
                </li>
              </ul>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="loading" class="loading">Loading...</div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useMeteoriteFilters } from '../features/meteorites/hooks/useMeteoriteFilters';
import { fetchMetadata, fetchMeteorites } from '../features/meteorites/services/useMeteoriteService';
import type { Meteorite, MeteoriteGroup } from '../features/meteorites/types/meteoriteTypes';

const { filters, resetFilters } = useMeteoriteFilters();

const items = ref<Meteorite[] | MeteoriteGroup[]>([]);
const yearOptions = ref<number[]>([]);
const classOptions = ref<string[]>([]);
const loading = ref(false);
const allDataLoaded = ref(false);

// 🧠 Типизированные computed-переменные
const meteoriteItems = computed(() => {
  return !filters.groupByYear ? (items.value as Meteorite[]) : [];
});

const groupedItems = computed(() => {
  return filters.groupByYear ? (items.value as MeteoriteGroup[]) : [];
});

const loadMetadata = async () => {
  try {
    const data = await fetchMetadata();
    yearOptions.value = data.years;
    classOptions.value = data.recclasses;
  } catch (error) {
    console.error('Error fetching metadata:', error);
  }
};

const loadMeteorites = async () => {
  if (loading.value || allDataLoaded.value) return;
  loading.value = true;
  try {
    const data = await fetchMeteorites(filters);
    if (data.items.length < filters.take) {
      allDataLoaded.value = true;
    }
    items.value = [...items.value, ...data.items];
    filters.skip += filters.take;
  } catch (error) {
    console.error('Error fetching meteorites:', error);
  } finally {
    loading.value = false;
  }
};

const resetAndFetchData = () => {
  items.value = [];
  filters.skip = 0;
  allDataLoaded.value = false;
  loadMeteorites();
};

const clearFilters = () => {
  resetFilters();
  resetAndFetchData();
};

const handleScroll = (event: Event) => {
  const target = event.target as HTMLElement;
  const { scrollTop, scrollHeight, clientHeight } = target;
  if (scrollTop + clientHeight >= scrollHeight - 10) {
    loadMeteorites();
  }
};

onMounted(async () => {
  await loadMetadata();
  loadMeteorites();
});
</script>


<style scoped>
.scroll-container {
  height: 80vh;
  overflow-y: auto;
  padding: 1rem;
  background: linear-gradient(to bottom, #000428, #004e92);
  background-size: cover;
  color: #fff;
  border: 1px solid #444;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.5);
}

.filters {
  margin-bottom: 1rem;
  padding: 1rem;
  background: rgba(0, 0, 0, 0.8);
  border: 1px solid #555;
  border-radius: 8px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.5);
}

.filters h2 {
  margin-bottom: 1rem;
  font-size: 1.8rem;
  color: #f9d71c;
  text-shadow: 0 0 5px #f9d71c, 0 0 10px #f9d71c;
}

.filter-group {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
}

.filter-group label {
  display: flex;
  flex-direction: column;
  color: #fff;
  font-weight: bold;
}

.filter-group select,
.filter-group input {
  padding: 0.5rem;
  border: 1px solid #555;
  border-radius: 4px;
  background: #222;
  color: #fff;
  box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.5);
}

.filter-buttons {
  margin-top: 1rem;
  display: flex;
  gap: 1rem;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
  transition: background-color 0.3s, transform 0.2s;
}

.btn-primary {
  background-color: #007bff;
  color: #fff;
  box-shadow: 0 2px 5px rgba(0, 123, 255, 0.5);
}

.btn-primary:hover {
  background-color: #0056b3;
  transform: scale(1.05);
}

.btn-secondary {
  background-color: #6c757d;
  color: #fff;
  box-shadow: 0 2px 5px rgba(108, 117, 125, 0.5);
}

.btn-secondary:hover {
  background-color: #495057;
  transform: scale(1.05);
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
  background: rgba(0, 0, 0, 0.8);
  border: 1px solid #555;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.5);
}

.data-table th,
.data-table td {
  border: 1px solid #444;
  padding: 0.8rem;
  text-align: left;
  color: #fff;
}

.data-table th {
  background: #222;
  color: #f9d71c;
  text-shadow: 0 0 5px #f9d71c, 0 0 10px #f9d71c;
  font-weight: bold;
}

.data-table tr:nth-child(even) {
  background: rgba(255, 255, 255, 0.05);
}

.data-table tr:hover {
  background: rgba(255, 255, 255, 0.1);
}

.loading {
  text-align: center;
  margin-top: 1rem;
  font-size: 1.5rem;
  color: #f9d71c;
  text-shadow: 0 0 5px #f9d71c, 0 0 10px #f9d71c;
}
</style>
