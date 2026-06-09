import type { ReactNode } from 'react';
import { getApiErrorDetails } from '@/api/apiClient';
import { EmptyState } from '@/components/common/EmptyState';
import { ErrorAlert } from '@/components/common/ErrorAlert';
import { LoadingState } from '@/components/common/LoadingState';

interface QueryStateProps<T> {
  isLoading: boolean;
  error: unknown;
  data: T | undefined;
  loadingMessage?: string;
  emptyTitle?: string;
  emptyDescription?: string;
  isEmpty?: (data: T) => boolean;
  children: (data: T) => ReactNode;
}

export function QueryState<T>({
  isLoading,
  error,
  data,
  loadingMessage,
  emptyTitle = 'Nenhum resultado encontrado',
  emptyDescription,
  isEmpty,
  children,
}: QueryStateProps<T>) {
  if (isLoading) {
    return <LoadingState message={loadingMessage} />;
  }

  if (error) {
    const details = getApiErrorDetails(error);
    return <ErrorAlert message={details.message} errors={details.errors} />;
  }

  if (data === undefined) {
    return <ErrorAlert message="Dados indisponíveis no momento." />;
  }

  if (isEmpty?.(data)) {
    return <EmptyState title={emptyTitle} description={emptyDescription} />;
  }

  return <>{children(data)}</>;
}
