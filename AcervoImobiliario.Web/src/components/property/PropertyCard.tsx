import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import {
  Box,
  Card,
  CardActionArea,
  CardContent,
  Stack,
  Typography,
} from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { StatusChip } from '@/components/ui/StatusChip';
import type { PropertyResponse } from '@/types/api';
import { formatAddress } from '@/utils/format';

interface PropertyCardProps {
  property: PropertyResponse;
}

export function PropertyCard({ property }: PropertyCardProps) {
  const complementLabel = property.complement?.trim() || null;

  return (
    <Card sx={{ height: '100%' }}>
      <CardActionArea
        component={RouterLink}
        to={`/imoveis/${property.id}`}
        sx={{ height: '100%', alignItems: 'stretch' }}
      >
        <CardContent sx={{ p: 2.5 }}>
          <Stack direction="row" justifyContent="space-between" alignItems="flex-start" spacing={1}>
            <Box sx={{ flex: 1 }}>
              <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 1 }}>
                <LocationOnIcon color="primary" fontSize="small" />
                <Typography variant="subtitle1">
                  {formatAddress(
                    property.street,
                    property.number,
                    property.neighborhood,
                    property.cityNameSnapshot,
                  )}
                </Typography>
              </Stack>

              {complementLabel ? (
                <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                  {complementLabel}
                </Typography>
              ) : null}

              {property.cadastralIndex ? (
                <Typography variant="caption" color="text.secondary" display="block">
                  Índice cadastral: {property.cadastralIndex}
                </Typography>
              ) : null}
            </Box>
            <ChevronRightIcon color="action" />
          </Stack>

          <Stack direction="row" spacing={1} sx={{ mt: 2 }}>
            <StatusChip active={property.isActive} />
          </Stack>
        </CardContent>
      </CardActionArea>
    </Card>
  );
}
