import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { queryKeys } from '@/hooks/queryKeys';
import { propertyHistoriesService } from '@/services/propertyHistoriesService';
import type { CreatePropertyHistoryRequest, HistorySortDirection } from '@/types/api';

export function usePropertyHistories(
  propertyId: string,
  sortDirection: HistorySortDirection = 'desc',
) {
  return useQuery({
    queryKey: queryKeys.histories.list(propertyId, sortDirection),
    queryFn: () => propertyHistoriesService.list(propertyId, sortDirection),
    enabled: Boolean(propertyId),
  });
}

export function useCreatePropertyHistory(propertyId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreatePropertyHistoryRequest) =>
      propertyHistoriesService.create(propertyId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.histories.all });
    },
  });
}
