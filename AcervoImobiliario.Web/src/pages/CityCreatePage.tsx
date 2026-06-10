import { Alert, Stack } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { getApiErrorDetails } from '@/api/apiClient';
import { ErrorAlert } from '@/components/common/ErrorAlert';
import { PageHeader } from '@/components/common/PageHeader';
import { CityForm } from '@/components/city/CityForm';
import { AppCard } from '@/components/ui/AppCard';
import { InfoAlert } from '@/components/ui/InfoAlert';
import { useCreateCity } from '@/hooks/useCities';
import type { CityFormValues } from '@/schemas/citySchema';

export function CityCreatePage() {
  const navigate = useNavigate();
  const createMutation = useCreateCity();

  const handleSubmit = (values: CityFormValues) => {
    createMutation.mutate(
      { name: values.name, state: values.state },
      {
        onSuccess: (city) => navigate(`/cidades/detalhes/${city.id}`, { state: { created: true } }),
      },
    );
  };

  const apiError = createMutation.error ? getApiErrorDetails(createMutation.error) : null;

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Nova cidade"
        subtitle="Informe o nome e o estado. Cidades duplicadas não são permitidas."
      />

      {createMutation.isSuccess ? (
        <Alert severity="success">Cidade cadastrada com sucesso.</Alert>
      ) : null}

      {apiError ? <ErrorAlert message={apiError.message} errors={apiError.errors} /> : null}

      <AppCard noHover>
        <InfoAlert sx={{ mb: 3 }}>
          Após o cadastro, a cidade ficará disponível para novos imóveis.
        </InfoAlert>

        <CityForm
          submitLabel="Salvar"
          isSubmitting={createMutation.isPending}
          onSubmit={handleSubmit}
          onCancel={() => navigate('/cidades')}
        />
      </AppCard>
    </Stack>
  );
}
