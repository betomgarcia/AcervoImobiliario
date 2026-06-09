import HistoryEduIcon from '@mui/icons-material/HistoryEdu';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import VisibilityIcon from '@mui/icons-material/Visibility';
import {
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  Stack,
  Typography,
} from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import type { PropertyResponse } from '@/types/api';
import { formatFullPropertyAddress } from '@/utils/format';

interface PropertySearchResultCardProps {
  property: PropertyResponse;
}

export function PropertySearchResultCard({ property }: PropertySearchResultCardProps) {
  const fullAddress = formatFullPropertyAddress(property);

  return (
    <Card sx={{ height: '100%' }}>
      <CardContent sx={{ p: 2.5, display: 'flex', flexDirection: 'column', height: '100%' }}>
        <Stack direction="row" spacing={1} alignItems="flex-start" sx={{ mb: 1.5 }}>
          <LocationOnIcon color="primary" sx={{ mt: 0.25 }} />
          <Box sx={{ flex: 1 }}>
            <Typography variant="subtitle1" fontWeight={700} sx={{ lineHeight: 1.4 }}>
              {fullAddress}
            </Typography>
            {property.cadastralIndex ? (
              <Typography variant="body2" color="text.secondary" sx={{ mt: 0.75 }}>
                Índice cadastral: {property.cadastralIndex}
              </Typography>
            ) : null}
          </Box>
        </Stack>

        <Chip
          size="small"
          label={property.isActive ? 'Ativo' : 'Inativo'}
          color={property.isActive ? 'success' : 'default'}
          variant="outlined"
          sx={{ alignSelf: 'flex-start', mb: 2 }}
        />

        <Stack
          direction={{ xs: 'column', sm: 'row' }}
          spacing={1}
          sx={{ mt: 'auto' }}
        >
          <Button
            component={RouterLink}
            to={`/imoveis/${property.id}`}
            variant="outlined"
            size="small"
            startIcon={<VisibilityIcon />}
            fullWidth
          >
            Ver detalhes
          </Button>
          <Button
            component={RouterLink}
            to={`/imoveis/${property.id}/historico`}
            variant="contained"
            size="small"
            startIcon={<HistoryEduIcon />}
            fullWidth
          >
            Ver histórico
          </Button>
        </Stack>
      </CardContent>
    </Card>
  );
}
