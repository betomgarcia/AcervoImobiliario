import type { HistorySortDirection, SearchPropertiesParams } from '@/types/api';

export const queryKeys = {
  cities: {
    all: ['cities'] as const,
    list: () => [...queryKeys.cities.all, 'list'] as const,
    search: (term: string) => [...queryKeys.cities.all, 'search', term] as const,
  },
  properties: {
    all: ['properties'] as const,
    detail: (id: string) => [...queryKeys.properties.all, 'detail', id] as const,
    search: (params: SearchPropertiesParams) =>
      [...queryKeys.properties.all, 'search', params] as const,
    neighborhoods: (cityId: string, term: string) =>
      [...queryKeys.properties.all, 'neighborhoods', cityId, term] as const,
    streets: (cityId: string, neighborhood: string, term: string) =>
      [...queryKeys.properties.all, 'streets', cityId, neighborhood, term] as const,
    numbers: (cityId: string, neighborhood: string, street: string, term: string) =>
      [...queryKeys.properties.all, 'numbers', cityId, neighborhood, street, term] as const,
  },
  histories: {
    all: ['histories'] as const,
    list: (propertyId: string, sortDirection: HistorySortDirection) =>
      [...queryKeys.histories.all, propertyId, sortDirection] as const,
  },
};
