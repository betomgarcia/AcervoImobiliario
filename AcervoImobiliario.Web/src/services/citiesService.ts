import { apiClient } from '@/api/apiClient';
import type {
  CityResponse,
  CityStatusFilter,
  CreateCityRequest,
  ListCitiesParams,
  UpdateCityRequest,
} from '@/types/api';

export const citiesService = {
  async list(params: ListCitiesParams = {}): Promise<CityResponse[]> {
    const { data } = await apiClient.get<CityResponse[]>('/api/cities', {
      params: {
        name: params.name?.trim() || undefined,
        status: params.status ?? 'Active',
      },
    });
    return data;
  },

  async listActive(): Promise<CityResponse[]> {
    return this.list({ status: 'Active' });
  },

  async getById(id: string): Promise<CityResponse> {
    const { data } = await apiClient.get<CityResponse>(`/api/cities/${id}`);
    return data;
  },

  async search(term: string): Promise<CityResponse[]> {
    const { data } = await apiClient.get<CityResponse[]>('/api/cities/search', {
      params: { term },
    });
    return data;
  },

  async create(request: CreateCityRequest): Promise<CityResponse> {
    const { data } = await apiClient.post<CityResponse>('/api/cities', {
      name: request.name.trim(),
      state: request.state.trim().toUpperCase(),
    });
    return data;
  },

  async update(id: string, request: UpdateCityRequest): Promise<CityResponse> {
    const { data } = await apiClient.put<CityResponse>(`/api/cities/${id}`, {
      name: request.name.trim(),
      state: request.state.trim().toUpperCase(),
      isActive: request.isActive,
    });
    return data;
  },

  async activate(id: string): Promise<CityResponse> {
    const { data } = await apiClient.post<CityResponse>(`/api/cities/${id}/activate`);
    return data;
  },

  async deactivate(id: string): Promise<CityResponse> {
    const { data } = await apiClient.post<CityResponse>(`/api/cities/${id}/deactivate`);
    return data;
  },
};

export type { CityStatusFilter };
