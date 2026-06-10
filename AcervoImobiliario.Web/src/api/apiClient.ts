import axios, { type AxiosError } from 'axios';
import type { ApiErrorResponse } from '@/types/api';

const baseURL = import.meta.env.VITE_API_BASE_URL ?? '';

export const apiClient = axios.create({
  baseURL,
  headers: { 'Content-Type': 'application/json' },
});

export class ApiRequestError extends Error {
  readonly status: number;
  readonly errors: string[];

  constructor(status: number, message: string, errors: string[]) {
    super(message);
    this.name = 'ApiRequestError';
    this.status = status;
    this.errors = errors;
  }
}

export function getApiErrorMessage(error: unknown): string {
  if (error instanceof ApiRequestError) {
    return error.errors.length > 0 ? error.errors.join(' ') : error.message;
  }

  if (axios.isAxiosError(error)) {
    const data = error.response?.data as ApiErrorResponse | undefined;
    if (data?.message) {
      return data.errors?.length ? data.errors.join(' ') : data.message;
    }
  }

  return 'Não foi possível concluir a operação. Tente novamente.';
}

export function getApiErrorDetails(error: unknown): { message: string; errors: string[] } {
  if (error instanceof ApiRequestError) {
    return { message: error.message, errors: error.errors };
  }

  return { message: getApiErrorMessage(error), errors: [] };
}

apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ApiErrorResponse>) => {
    const status = error.response?.status ?? 500;
    const data = error.response?.data;

    if (data?.success === false) {
      throw new ApiRequestError(status, data.message, data.errors ?? []);
    }

    throw error;
  },
);
