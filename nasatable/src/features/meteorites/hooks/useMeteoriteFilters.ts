import { reactive } from 'vue';
import type { MeteoriteFilters } from '../types/meteoriteTypes';

export const useMeteoriteFilters = () => {
  const filters = reactive<MeteoriteFilters>({
    search: '',
    meteoriteClass: '',
    yearFrom: '',
    yearTo: '',
    sortBy: 'name',
    sortDescending: false,
    groupByYear: false,
    skip: 0,
    take: 20,
  });

  const resetFilters = () => {
    filters.search = '';
    filters.meteoriteClass = '';
    filters.yearFrom = '';
    filters.yearTo = '';
    filters.sortBy = 'name';
    filters.sortDescending = false;
    filters.groupByYear = false;
    filters.skip = 0;
  };

  return { filters, resetFilters };
};