import { apiClient } from '@/api/apiClient';
import type {
  CreatePropertyRequest,
  PropertyResponse,
  SearchPropertiesParams,
  UpdatePropertyRequest,
} from '@/types/api';

export const propertiesService = {
  async search(params: SearchPropertiesParams): Promise<PropertyResponse[]> {
    const { data } = await apiClient.get<PropertyResponse[]>('/api/properties/search', {
      params,
    });
    return data;
  },

  async getById(id: string): Promise<PropertyResponse> {
    const { data } = await apiClient.get<PropertyResponse>(`/api/properties/${id}`);
    return data;
  },

  async create(request: CreatePropertyRequest): Promise<PropertyResponse> {
    const { data } = await apiClient.post<PropertyResponse>('/api/properties', request);
    return data;
  },

  async update(id: string, request: UpdatePropertyRequest): Promise<PropertyResponse> {
    const { data } = await apiClient.put<PropertyResponse>(`/api/properties/${id}`, request);
    return data;
  },

  async searchNeighborhoods(cityId: string, term: string): Promise<string[]> {
    const { data } = await apiClient.get<string[]>('/api/properties/neighborhoods/search', {
      params: { cityId, term },
    });
    return data;
  },

  async searchStreets(
    cityId: string,
    neighborhood: string,
    term: string,
  ): Promise<string[]> {
    const { data } = await apiClient.get<string[]>('/api/properties/streets/search', {
      params: { cityId, neighborhood, term },
    });
    return data;
  },

  async searchNumbers(
    cityId: string,
    neighborhood: string,
    street: string,
    term: string,
  ): Promise<string[]> {
    const { data } = await apiClient.get<string[]>('/api/properties/numbers/search', {
      params: { cityId, neighborhood, street, term },
    });
    return data;
  },
};
