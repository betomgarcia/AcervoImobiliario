import { zodResolver } from '@hookform/resolvers/zod';
import {
  Box,
  Button,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
} from '@mui/material';
import type { Control, FieldErrors } from 'react-hook-form';
import { Controller, useForm } from 'react-hook-form';
import { Link as RouterLink, useNavigate, useParams } from 'react-router-dom';
import { getApiErrorDetails } from '@/api/apiClient';
import { ErrorAlert } from '@/components/common/ErrorAlert';
import { QueryState } from '@/components/common/QueryState';
import { PageHeader } from '@/components/common/PageHeader';
import { PropertySummaryCard } from '@/components/property/PropertySummaryCard';
import { AppCard } from '@/components/ui/AppCard';
import { InfoAlert } from '@/components/ui/InfoAlert';
import { useProperty } from '@/hooks/useProperties';
import { useCreatePropertyHistory } from '@/hooks/usePropertyHistories';
import { historyFormSchema, type HistoryFormValues } from '@/schemas/historySchema';
import { PropertyHistoryEventType } from '@/types/api';
import { formatAddress, fromIsoDateTimeLocal, toIsoDateTimeLocal } from '@/utils/format';
import { historyEventTypeLabels } from '@/utils/labels';

const eventTypeOptions = Object.entries(historyEventTypeLabels).map(([value, label]) => ({
  value: Number(value) as PropertyHistoryEventType,
  label,
}));

export function PropertyHistoryCreatePage() {
  const { id = '' } = useParams();
  const navigate = useNavigate();

  const propertyQuery = useProperty(id);
  const createMutation = useCreatePropertyHistory(id);

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<HistoryFormValues>({
    resolver: zodResolver(historyFormSchema),
    defaultValues: {
      eventType: PropertyHistoryEventType.Note,
      eventDate: toIsoDateTimeLocal(new Date()),
      description: '',
    },
  });

  const apiError = createMutation.error
    ? getApiErrorDetails(createMutation.error)
    : null;

  return (
    <QueryState
      isLoading={propertyQuery.isLoading}
      error={propertyQuery.error}
      data={propertyQuery.data}
      loadingMessage="Carregando imóvel..."
      emptyTitle="Imóvel não encontrado"
      emptyDescription="Não é possível registrar histórico para um imóvel inexistente."
      isEmpty={(property) => !property}
    >
      {(property) => (
        <Stack spacing={3}>
          <PageHeader
            title="Adicionar histórico"
            subtitle={formatAddress(
              property.street,
              property.number,
              property.neighborhood,
              property.cityNameSnapshot,
            )}
          />

          <PropertySummaryCard property={property} />

          {apiError ? (
            <ErrorAlert message={apiError.message} errors={apiError.errors} />
          ) : null}

          <AppCard noHover>
            <InfoAlert sx={{ mb: 3 }}>
              Históricos não podem ser editados ou apagados. Em caso de erro, registre uma
              correção.
            </InfoAlert>

            <HistoryForm
              control={control}
              errors={errors}
              isSubmitting={createMutation.isPending}
              onSubmit={handleSubmit((values) =>
                createMutation.mutate(
                  {
                    eventType: values.eventType,
                    eventDate: fromIsoDateTimeLocal(values.eventDate),
                    description: values.description.trim(),
                  },
                  { onSuccess: () => navigate(`/imoveis/${id}/historico`) },
                ),
              )}
              propertyId={id}
            />
          </AppCard>
        </Stack>
      )}
    </QueryState>
  );
}

function HistoryForm({
  control,
  errors,
  isSubmitting,
  onSubmit,
  propertyId,
}: {
  control: Control<HistoryFormValues>;
  errors: FieldErrors<HistoryFormValues>;
  isSubmitting: boolean;
  onSubmit: () => void;
  propertyId: string;
}) {
  return (
    <Box component="form" onSubmit={onSubmit} noValidate>
      <Grid container spacing={2.5}>
        <Grid item xs={12} md={4}>
          <Controller
            name="eventType"
            control={control}
            render={({ field }) => (
              <FormControl fullWidth error={Boolean(errors.eventType)}>
                <InputLabel>Tipo do evento</InputLabel>
                <Select {...field} label="Tipo do evento">
                  {eventTypeOptions.map((option) => (
                    <MenuItem key={option.value} value={option.value}>
                      {option.label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            )}
          />
        </Grid>

        <Grid item xs={12} md={8}>
          <Controller
            name="eventDate"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                type="datetime-local"
                label="Data do evento"
                InputLabelProps={{ shrink: true }}
                error={Boolean(errors.eventDate)}
                helperText={errors.eventDate?.message}
              />
            )}
          />
        </Grid>

        <Grid item xs={12}>
          <Controller
            name="description"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                multiline
                minRows={5}
                label="Descrição"
                placeholder="Descreva o que aconteceu neste evento..."
                error={Boolean(errors.description)}
                helperText={errors.description?.message}
              />
            )}
          />
        </Grid>
      </Grid>

      <Stack
        direction={{ xs: 'column', sm: 'row' }}
        spacing={1.5}
        justifyContent="flex-end"
        sx={{ mt: 3 }}
      >
        <Button
          component={RouterLink}
          to={`/imoveis/${propertyId}/historico`}
          variant="outlined"
          color="primary"
        >
          Cancelar
        </Button>
        <Button type="submit" variant="contained" color="primary" disabled={isSubmitting}>
          {isSubmitting ? 'Salvando...' : 'Salvar histórico'}
        </Button>
      </Stack>
    </Box>
  );
}
