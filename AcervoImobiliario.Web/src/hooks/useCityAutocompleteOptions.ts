import { useMemo } from 'react';
import { useActiveCities } from '@/hooks/useCities';
import type { CityResponse } from '@/types/api';

function normalizeSearchTerm(term: string): string {
  return term
    .trim()
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '');
}

function filterCitiesByTerm(cities: CityResponse[], term: string): CityResponse[] {
  const normalized = normalizeSearchTerm(term);
  if (!normalized) {
    return cities;
  }

  return cities.filter(
    (city) =>
      city.nameNormalized.includes(normalized) ||
      city.name.toLowerCase().includes(normalized) ||
      city.state.toLowerCase().includes(normalized),
  );
}

/** Filtra localmente a lista já carregada de cidades ativas (sem busca na API). */
export function useCityAutocompleteOptions(inputValue: string) {
  const { data: allCities = [], isLoading } = useActiveCities();

  const cityOptions = useMemo(() => {
    if (inputValue.trim().length < 2) {
      return allCities;
    }
    return filterCitiesByTerm(allCities, inputValue);
  }, [allCities, inputValue]);

  const resolveCity = (cityId: string | undefined): CityResponse | null => {
    if (!cityId) {
      return null;
    }

    return allCities.find((city) => city.id === cityId) ?? null;
  };

  return {
    cityOptions,
    resolveCity,
    isSearching: isLoading,
  };
}
