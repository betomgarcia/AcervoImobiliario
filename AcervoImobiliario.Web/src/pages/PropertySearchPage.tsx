import { Alert, Grid, Stack } from '@mui/material';
import { useState } from 'react';
import { getApiErrorDetails } from '@/api/apiClient';
import { EmptyState } from '@/components/common/EmptyState';
import { ErrorAlert } from '@/components/common/ErrorAlert';
import { LoadingState } from '@/components/common/LoadingState';
import { PageHeader } from '@/components/common/PageHeader';
import { usePwaInstallBottomOffset } from '@/components/pwa/PwaInstallContext';
import {
  PropertySearchForm,
  type PropertySearchFilters,
} from '@/components/property/PropertySearchForm';
import { PropertySearchResultCard } from '@/components/property/PropertySearchResultCard';
import { useSearchPropertiesMutation } from '@/hooks/useProperties';
import type { PropertyResponse } from '@/types/api';

function buildEmptyMessage(filters: PropertySearchFilters | null): {
  title: string;
  description: string;
} {
  if (!filters) {
    return {
      title: 'Nenhum imóvel encontrado',
      description: 'Tente ajustar os filtros e buscar novamente.',
    };
  }

  if (filters.mode === 'cadastral') {
    return {
      title: 'Nenhum imóvel com este índice cadastral',
      description: `Não encontramos imóveis para o índice "${filters.cadastralIndex}". Verifique se o código está correto ou cadastre um novo imóvel.`,
    };
  }

  const parts = [
    filters.cityName,
    filters.neighborhood,
    filters.street,
    filters.number,
  ].filter(Boolean);

  const location = parts.length > 0 ? parts.join(' · ') : 'os filtros informados';

  return {
    title: 'Nenhum imóvel encontrado neste endereço',
    description: `Não há imóveis cadastrados para ${location}. Você pode refinar bairro, rua, número ou complemento, ou cadastrar um novo imóvel.`,
  };
}

export function PropertySearchPage() {
  const [results, setResults] = useState<PropertyResponse[]>([]);
  const [hasSearched, setHasSearched] = useState(false);
  const [lastFilters, setLastFilters] = useState<PropertySearchFilters | null>(null);
  const pwaBottomOffset = usePwaInstallBottomOffset();

  const searchMutation = useSearchPropertiesMutation();

  const handleSearch = (filters: PropertySearchFilters) => {
    setLastFilters(filters);

    const params =
      filters.mode === 'cadastral'
        ? { cadastralIndex: filters.cadastralIndex }
        : {
            cityId: filters.cityId,
            neighborhood: filters.neighborhood || undefined,
            street: filters.street || undefined,
            number: filters.number || undefined,
            complement: filters.complement,
          };

    searchMutation.mutate(params, {
      onSuccess: (data) => {
        setResults(data);
        setHasSearched(true);
      },
    });
  };

  const searchError = searchMutation.error
    ? getApiErrorDetails(searchMutation.error)
    : null;

  const emptyMessage = buildEmptyMessage(lastFilters);

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Buscar imóveis"
        subtitle="Busca progressiva por endereço ou localização direta por índice cadastral."
      />

      <PropertySearchForm
        onSearch={handleSearch}
        isSearching={searchMutation.isPending}
      />

      {searchError ? (
        <ErrorAlert message={searchError.message} errors={searchError.errors} />
      ) : null}

      {searchMutation.isPending ? <LoadingState message="Buscando imóveis na API..." /> : null}

      {hasSearched && !searchMutation.isPending && !searchError ? (
        results.length > 0 ? (
          <>
            <Alert severity="success" variant="standard" sx={{ borderRadius: 2 }}>
              Encontramos {results.length}{' '}
              {results.length === 1 ? 'imóvel' : 'imóveis'} para sua busca.
            </Alert>
            <Grid
              container
              spacing={2}
              sx={{
                pb: pwaBottomOffset > 0 ? `${pwaBottomOffset}px` : undefined,
                transition: 'padding-bottom 0.2s ease',
              }}
            >
              {results.map((property) => (
                <Grid item xs={12} md={6} lg={4} key={property.id}>
                  <PropertySearchResultCard property={property} />
                </Grid>
              ))}
            </Grid>
          </>
        ) : (
          <EmptyState
            title={emptyMessage.title}
            description={emptyMessage.description}
          />
        )
      ) : null}
    </Stack>
  );
}
