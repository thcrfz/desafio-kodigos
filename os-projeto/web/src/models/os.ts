export interface UsuarioMin {
  id: number;
  nome: string;
  email: string;
}

export type StatusOS = 0 | 1 | 2; // 0=Aberta, 1=EmExecucao, 2=Fechada

export interface OsListItem {
  id: number;
  titulo: string;
  status: StatusOS;
  tecnicoId: number | null;
  tecnico: UsuarioMin | null;
  createdAt: string; // ISO
  updatedAt: string; // ISO
}
