import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { queryKeys } from '@/hooks/queryKeys';
import { propertiesService } from '@/services/propertiesService';
import type {
  CreatePropertyRequest,
  SearchPropertiesParams,
  UpdatePropertyRequest,
} from '@/types/api';

export function useProperty(id: string) {
  return useQuery({
    queryKey: queryKeys.properties.detail(id),
    queryFn: () => propertiesService.getById(id),
    enabled: Boolean(id),
  });
}

export function useSearchProperties(params: SearchPropertiesParams | null) {
  return useQuery({
    queryKey: queryKeys.properties.search(params ?? {}),
    queryFn: () => propertiesService.search(params!),
    enabled: Boolean(params?.cityId || params?.cadastralIndex),
  });
}

export function useSearchPropertiesMutation() {
  return useMutation({
    mutationFn: (params: SearchPropertiesParams) => propertiesService.search(params),
  });
}

export function useCreateProperty() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreatePropertyRequest) => propertiesService.create(request),
    onSuccess: (property) => {
      queryClient.setQueryData(queryKeys.properties.detail(property.id), property);
      queryClient.invalidateQueries({ queryKey: queryKeys.properties.all });
    },
  });
}

export function useUpdateProperty(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: UpdatePropertyRequest) => propertiesService.update(id, request),
    onSuccess: (property) => {
      queryClient.setQueryData(queryKeys.properties.detail(property.id), property);
      queryClient.invalidateQueries({ queryKey: queryKeys.properties.all });
    },
  });
}

export function useSearchNeighborhoods(cityId: string, term: string) {
  const normalizedTerm = term.trim();

  return useQuery({
    queryKey: queryKeys.properties.neighborhoods(cityId, normalizedTerm),
    queryFn: () => propertiesService.searchNeighborhoods(cityId, normalizedTerm),
    enabled: Boolean(cityId) && normalizedTerm.length >= 2,
  });
}

export function useSearchStreets(cityId: string, neighborhood: string, term: string) {
  const normalizedTerm = term.trim();

  return useQuery({
    queryKey: queryKeys.properties.streets(cityId, neighborhood, normalizedTerm),
    queryFn: () => propertiesService.searchStreets(cityId, neighborhood, normalizedTerm),
    enabled: Boolean(cityId && neighborhood) && normalizedTerm.length >= 2,
  });
}

export function useSearchNumbers(
  cityId: string,
  neighborhood: string,
  street: string,
  term: string,
) {
  const normalizedTerm = term.trim();

  return useQuery({
    queryKey: queryKeys.properties.numbers(cityId, neighborhood, street, normalizedTerm),
    queryFn: () => propertiesService.searchNumbers(cityId, neighborhood, street, normalizedTerm),
    enabled: Boolean(cityId && neighborhood && street) && normalizedTerm.length >= 1,
  });
}
