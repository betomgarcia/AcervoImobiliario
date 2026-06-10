import ApartmentIcon from '@mui/icons-material/Apartment';
import HomeWorkIcon from '@mui/icons-material/HomeWork';
import LocationCityIcon from '@mui/icons-material/LocationCity';
import MenuIcon from '@mui/icons-material/Menu';
import SearchIcon from '@mui/icons-material/Search';
import {
  AppBar,
  Box,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography,
  useMediaQuery,
  useTheme,
} from '@mui/material';
import { useState } from 'react';
import { NavLink, Outlet } from 'react-router-dom';
import { PageContainer } from '@/components/ui/PageContainer';
import { tokens } from '@/theme/tokens';

const drawerWidth = tokens.layout.drawerWidth;

const navItems = [
  { to: '/', label: 'Buscar imóveis', icon: <SearchIcon /> },
  { to: '/imoveis/novo', label: 'Cadastrar imóvel', icon: <HomeWorkIcon /> },
  { to: '/cidades', label: 'Cidades', icon: <LocationCityIcon /> },
];

export function AppLayout() {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const [mobileOpen, setMobileOpen] = useState(false);

  const drawer = (
    <Box sx={{ py: 2.5 }}>
      <Box sx={{ px: 2.5, pb: 2 }}>
        <Typography variant="overline">Navegação</Typography>
      </Box>
      <List sx={{ px: 1.5 }}>
        {navItems.map((item) => (
          <ListItemButton
            key={item.to}
            component={NavLink}
            to={item.to}
            end={item.to === '/'}
            onClick={() => setMobileOpen(false)}
            sx={{
              mx: 0.5,
              mb: 0.75,
              py: 1.35,
              borderRadius: `${tokens.radius.md}px`,
              color: tokens.color.textPrimary,
              transition: tokens.transition.nav,
              '& .MuiListItemIcon-root': {
                color: tokens.color.textSecondary,
                transition: tokens.transition.nav,
              },
              '&:hover': {
                bgcolor: tokens.color.navHover,
              },
              '&.active': {
                bgcolor: tokens.color.primary,
                color: tokens.color.textOnPrimary,
                '& .MuiListItemIcon-root': { color: tokens.color.textOnPrimary },
                '&:hover': { bgcolor: tokens.color.primaryDark },
              },
            }}
          >
            <ListItemIcon sx={{ minWidth: 40 }}>{item.icon}</ListItemIcon>
            <ListItemText
              primary={item.label}
              primaryTypographyProps={{ fontWeight: 600, fontSize: '0.9375rem' }}
            />
          </ListItemButton>
        ))}
      </List>
    </Box>
  );

  return (
    <Box sx={{ display: 'flex', minHeight: '100dvh', bgcolor: 'background.default' }}>
      <AppBar
        position="fixed"
        elevation={0}
        sx={{
          width: { md: `calc(100% - ${drawerWidth}px)` },
          ml: { md: `${drawerWidth}px` },
        }}
      >
        <Toolbar sx={{ minHeight: { xs: 60, sm: 68 }, gap: 1.5, px: { xs: 2, sm: 3 } }}>
          {isMobile ? (
            <IconButton
              color="inherit"
              edge="start"
              onClick={() => setMobileOpen(true)}
              aria-label="Abrir menu"
              sx={{ color: tokens.color.textOnPrimary }}
            >
              <MenuIcon />
            </IconButton>
          ) : null}
          <ApartmentIcon
            sx={{
              fontSize: { xs: 28, sm: 32 },
              flexShrink: 0,
              color: tokens.color.accent,
            }}
          />
          <Box sx={{ minWidth: 0, flex: 1 }}>
            <Typography
              variant="h6"
              component="div"
              noWrap
              sx={{
                fontSize: { xs: '1.1rem', sm: '1.3rem' },
                fontWeight: 700,
                lineHeight: 1.25,
                color: tokens.color.textOnPrimary,
              }}
            >
              Acervo Imobiliário
            </Typography>
            <Typography
              component="p"
              variant="body2"
              sx={{
                mt: 0.25,
                fontWeight: 500,
                fontSize: { xs: '0.75rem', sm: '0.875rem' },
                lineHeight: 1.35,
                color: tokens.color.textAccentOnPrimary,
              }}
            >
              Endereço único e histórico de eventos
            </Typography>
          </Box>
        </Toolbar>
      </AppBar>

      <Box component="nav" sx={{ width: { md: drawerWidth }, flexShrink: { md: 0 } }}>
        <Drawer
          variant={isMobile ? 'temporary' : 'permanent'}
          open={isMobile ? mobileOpen : true}
          onClose={() => setMobileOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{
            '& .MuiDrawer-paper': {
              width: drawerWidth,
              boxSizing: 'border-box',
            },
          }}
        >
          <Toolbar sx={{ minHeight: { xs: 60, sm: 68 } }} />
          {drawer}
        </Drawer>
      </Box>

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          width: { md: `calc(100% - ${drawerWidth}px)` },
          px: { xs: 1.5, sm: 2.5, md: 3 },
          py: { xs: 2, md: 3 },
          pb: `calc(${theme.spacing(2.5)} + env(safe-area-inset-bottom, 0px))`,
        }}
      >
        <Toolbar sx={{ minHeight: { xs: 60, sm: 68 } }} />
        <PageContainer>
          <Outlet />
        </PageContainer>
      </Box>
    </Box>
  );
}
