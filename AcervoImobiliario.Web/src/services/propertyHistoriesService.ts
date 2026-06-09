import { apiClient } from '@/api/apiClient';
import type {
  CreatePropertyHistoryRequest,
  HistorySortDirection,
  PropertyHistoryResponse,
} from '@/types/api';

export const propertyHistoriesService = {
  async list(
    propertyId: string,
    sortDirection: HistorySortDirection = 'desc',
  ): Promise<PropertyHistoryResponse[]> {
    const { data } = await apiClient.get<PropertyHistoryResponse[]>(
      `/api/properties/${propertyId}/histories`,
      { params: { sortDirection } },
    );
    return data;
  },

  async create(
    propertyId: string,
    request: CreatePropertyHistoryRequest,
  ): Promise<PropertyHistoryResponse> {
    const { data } = await apiClient.post<PropertyHistoryResponse>(
      `/api/properties/${propertyId}/histories`,
      request,
    );
    return data;
  },
};
