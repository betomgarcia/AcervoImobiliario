import { useQuery } from '@tanstack/react-query';
import { citiesService } from '@/services/citiesService';
import { queryKeys } from '@/hooks/queryKeys';

export function useActiveCities() {
  return useQuery({
    queryKey: queryKeys.cities.list(),
    queryFn: () => citiesService.listActive(),
  });
}

export function useSearchCities(term: string, enabled = true) {
  const normalizedTerm = term.trim();

  return useQuery({
    queryKey: queryKeys.cities.search(normalizedTerm),
    queryFn: () => citiesService.search(normalizedTerm),
    enabled: enabled && normalizedTerm.length >= 2,
  });
}
