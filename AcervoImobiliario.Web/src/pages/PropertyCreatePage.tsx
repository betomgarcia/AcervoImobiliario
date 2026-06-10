import { Alert, Stack } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { getApiErrorDetails } from '@/api/apiClient';
import { ErrorAlert } from '@/components/common/ErrorAlert';
import { PageHeader } from '@/components/common/PageHeader';
import { PropertyForm } from '@/components/property/PropertyForm';
import { AppCard } from '@/components/ui/AppCard';
import { InfoAlert } from '@/components/ui/InfoAlert';
import { useCreateProperty } from '@/hooks/useProperties';
import type { PropertyFormValues } from '@/schemas/propertySchema';

export function PropertyCreatePage() {
  const navigate = useNavigate();
  const createMutation = useCreateProperty();

  const handleSubmit = (values: PropertyFormValues) => {
    createMutation.mutate(
      {
        cityId: values.cityId,
        neighborhood: values.neighborhood.trim(),
        street: values.street.trim(),
        number: values.number.trim(),
        complement: values.complement?.trim() || null,
        cadastralIndex: values.cadastralIndex?.trim() || null,
      },
      {
        onSuccess: (property) => navigate(`/imoveis/${property.id}`),
      },
    );
  };

  const apiError = createMutation.error
    ? getApiErrorDetails(createMutation.error)
    : null;

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Cadastrar imóvel"
        subtitle="Informe o endereço completo. O sistema impede duplicidade por endereço único."
      />

      {createMutation.isSuccess ? (
        <Alert severity="success">Imóvel cadastrado com sucesso. Redirecionando...</Alert>
      ) : null}

      {apiError ? <ErrorAlert message={apiError.message} errors={apiError.errors} /> : null}

      <AppCard noHover>
        <InfoAlert sx={{ mb: 3 }}>
          Complemento é texto livre (ex.: Apto 303 Bloco A). Número aceita somente dígitos.
        </InfoAlert>

        <PropertyForm
          submitLabel="Cadastrar imóvel"
          isSubmitting={createMutation.isPending}
          onSubmit={handleSubmit}
        />
      </AppCard>
    </Stack>
  );
}
