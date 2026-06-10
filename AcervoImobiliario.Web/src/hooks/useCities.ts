import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { citiesService } from '@/services/citiesService';
import { queryKeys } from '@/hooks/queryKeys';
import type {
  CityStatusFilter,
  CreateCityRequest,
  ListCitiesParams,
  UpdateCityRequest,
} from '@/types/api';

export function useActiveCities() {
  return useQuery({
    queryKey: queryKeys.cities.list('Active'),
    queryFn: () => citiesService.listActive(),
  });
}

export function useCities(params: ListCitiesParams) {
  return useQuery({
    queryKey: queryKeys.cities.list(params.status ?? 'All', params.name),
    queryFn: () => citiesService.list(params),
  });
}

export function useCity(id: string) {
  return useQuery({
    queryKey: queryKeys.cities.detail(id),
    queryFn: () => citiesService.getById(id),
    enabled: Boolean(id),
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

export function useCreateCity() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateCityRequest) => citiesService.create(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.cities.all });
    },
  });
}

export function useUpdateCity(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: UpdateCityRequest) => citiesService.update(id, request),
    onSuccess: (city) => {
      queryClient.setQueryData(queryKeys.cities.detail(city.id), city);
      queryClient.invalidateQueries({ queryKey: queryKeys.cities.all });
    },
  });
}

export function useActivateCity() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => citiesService.activate(id),
    onSuccess: (city) => {
      queryClient.setQueryData(queryKeys.cities.detail(city.id), city);
      queryClient.invalidateQueries({ queryKey: queryKeys.cities.all });
    },
  });
}

export function useDeactivateCity() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => citiesService.deactivate(id),
    onSuccess: (city) => {
      queryClient.setQueryData(queryKeys.cities.detail(city.id), city);
      queryClient.invalidateQueries({ queryKey: queryKeys.cities.all });
    },
  });
}

export type { CityStatusFilter };
