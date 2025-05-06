import axios from 'axios';
import type { MeteoriteFilters } from '../types/meteoriteTypes';

//const API_URL = import.meta.env.VITE_API_URL; // get some weird error when using this
const apiBase = window.location.hostname === 'localhost'
  ? 'http://localhost:5000'
  : 'http://backend:80';

export const fetchMetadata = async () => {
  const { data } = await axios.get(`${apiBase}/api/meteorite/metadata`);
  return data;
};

export const fetchMeteorites = async (filters: MeteoriteFilters) => {
  const endpoint = filters.groupByYear ? 'grouped' : 'list';
  const { data } = await axios.get(`${apiBase}/api/meteorite/${endpoint}`, {
    params: filters
  });
  return data;
};
