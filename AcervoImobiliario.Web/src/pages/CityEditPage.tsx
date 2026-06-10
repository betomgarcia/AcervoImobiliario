import { Alert, Stack } from '@mui/material';
import { useNavigate, useParams } from 'react-router-dom';
import { getApiErrorDetails } from '@/api/apiClient';
import { ErrorAlert } from '@/components/common/ErrorAlert';
import { LoadingState } from '@/components/common/LoadingState';
import { PageHeader } from '@/components/common/PageHeader';
import { QueryState } from '@/components/common/QueryState';
import { CityForm } from '@/components/city/CityForm';
import { AppCard } from '@/components/ui/AppCard';
import { InfoAlert } from '@/components/ui/InfoAlert';
import { useCity, useUpdateCity } from '@/hooks/useCities';
import type { CityFormValues } from '@/schemas/citySchema';

export function CityEditPage() {
  const { id = '' } = useParams();
  const navigate = useNavigate();
  const cityQuery = useCity(id);
  const updateMutation = useUpdateCity(id);

  const apiError = updateMutation.error ? getApiErrorDetails(updateMutation.error) : null;

  if (cityQuery.isLoading) {
    return <LoadingState message="Carregando cidade..." />;
  }

  return (
    <QueryState
      isLoading={false}
      error={cityQuery.error}
      data={cityQuery.data}
      loadingMessage="Carregando cidade..."
      emptyTitle="Cidade não encontrada"
      emptyDescription="A cidade solicitada não existe."
      isEmpty={(city) => !city}
    >
      {(city) => {
        const handleSubmit = (values: CityFormValues) => {
          updateMutation.mutate(
            {
              name: values.name,
              state: values.state,
              isActive: city.isActive,
            },
            {
              onSuccess: () =>
                navigate(`/cidades/detalhes/${city.id}`, { state: { updated: true } }),
            },
          );
        };

        return (
          <Stack spacing={3}>
            <PageHeader
              title="Editar cidade"
              subtitle={`${city.name} — ${city.state}`}
            />

            {updateMutation.isSuccess ? (
              <Alert severity="success">Cidade atualizada com sucesso.</Alert>
            ) : null}

            {apiError ? <ErrorAlert message={apiError.message} errors={apiError.errors} /> : null}

            <AppCard noHover>
              <InfoAlert sx={{ mb: 3 }}>
                Para ativar ou inativar, use a listagem de cidades.
              </InfoAlert>

              <CityForm
                defaultValues={{ name: city.name, state: city.state }}
                submitLabel="Salvar"
                isSubmitting={updateMutation.isPending}
                showStatus
                isActive={city.isActive}
                onSubmit={handleSubmit}
                onCancel={() => navigate(`/cidades/detalhes/${city.id}`)}
              />
            </AppCard>
          </Stack>
        );
      }}
    </QueryState>
  );
}
