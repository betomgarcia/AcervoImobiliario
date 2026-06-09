import { apiClient } from '@/api/apiClient';
import type { CityResponse } from '@/types/api';

export const citiesService = {
  async listActive(): Promise<CityResponse[]> {
    const { data } = await apiClient.get<CityResponse[]>('/api/cities');
    return data;
  },

  async search(term: string): Promise<CityResponse[]> {
    const { data } = await apiClient.get<CityResponse[]>('/api/cities/search', {
      params: { term },
    });
    return data;
  },
};
