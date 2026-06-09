import HomeWorkIcon from '@mui/icons-material/HomeWork';
import { Alert, Card, CardContent, Stack } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { getApiErrorDetails } from '@/api/apiClient';
import { ErrorAlert } from '@/components/common/ErrorAlert';
import { PageHeader } from '@/components/common/PageHeader';
import { PropertyForm } from '@/components/property/PropertyForm';
import { useCreateProperty } from '@/hooks/useProperties';
import type { PropertyFormValues } from '@/schemas/propertySchema';
import { ComplementType } from '@/types/api';

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
        complementType: values.complementType,
        complementValue:
          values.complementType === ComplementType.None
            ? null
            : values.complementValue?.trim() || null,
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
        <Alert severity="success" sx={{ borderRadius: 2 }}>
          Imóvel cadastrado com sucesso. Redirecionando...
        </Alert>
      ) : null}

      {apiError ? <ErrorAlert message={apiError.message} errors={apiError.errors} /> : null}

      <Card>
        <CardContent sx={{ p: { xs: 2, md: 3 } }}>
          <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 3 }}>
            <HomeWorkIcon color="primary" />
            <Alert severity="info" sx={{ flex: 1, borderRadius: 2 }}>
              Apartamento, sala e loja exigem valor de complemento. Número aceita somente dígitos.
            </Alert>
          </Stack>

          <PropertyForm
            submitLabel="Cadastrar imóvel"
            isSubmitting={createMutation.isPending}
            onSubmit={handleSubmit}
          />
        </CardContent>
      </Card>
    </Stack>
  );
}
