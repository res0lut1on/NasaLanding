export interface Meteorite {
  id: number;
  name: string;
  year: number;
  mass: number;
  class: string;
}

export interface MeteoriteFilters {
  search: string;
  meteoriteClass: string;
  yearFrom: string;
  yearTo: string;
  sortBy: string;
  sortDescending: boolean;
  groupByYear: boolean;
  skip: number;
  take: number;
}

export interface MeteoriteGroup {
  year: number;
  totalCount: number;
  totalMass: number;
  meteorites: Meteorite[];
}