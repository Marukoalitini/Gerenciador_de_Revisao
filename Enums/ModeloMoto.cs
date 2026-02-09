namespace Motos.Enums;

/// <summary>
/// Modelos de motos disponíveis no sistema.
/// Valores mapeiam diretamente com os nomes no checklists-template.json
/// </summary>
public enum ModeloMoto
{
	FACTOR_125_I_UBS_2017_2025,
	FACTOR_150_UBS_2016_2024,
	NOVA_FACTOR_2025,
	NOVA_FACTOR_DX_2025,
	FAZER_150_UBS_2014_2025,
	FZ15_2023_2024,
	FAZER_FZ15_ABS_CONNECTED_2025_2026,
	FAZER_250_2018_2024,
	FZ25_CONNECTED_2025_2026,
	CROSSER_XTZ150_S_2015_2024,
	CROSSER_XTZ150_S_2025,
	CROSSER_XTZ150_Z_2015_2024,
	CROSSER_XTZ150_Z_2025,
	LANDER_250_2020_2024,
	LANDER_250_ABS_CONNECTED_2025,
	TENERE_700,
	R15_ABS_2024_2026,
	YZF_R3_ABS_2020_2025,
	YZF_R3_ABS_CONNECTED_2026,
	MT03_ABS_2021_2025,
	NOVA_MT03_CONNECTED_2026,
	NOVA_MT07_CONNECTED_2026,
	NEO_125_2017_2024,
	NEOS_DUAL_CONNECTED_2026,
	FLUO_ABS_2022_2025,
	FLUO_ABS_HYBRID_CONNECTED_2026,
	AEROX_ABS_CONNECTED,
	NMAX_CONNECTED_2023_2024,
	NOVA_NMAX_CONNECTED_2025,
	XMAX_2021_2024,
	XMAX_CONNECTED_2025,
	NOVA_XMAX_300_CONNECTED_2026
}

public static class ModeloMotoExtensions
{
	private static readonly Dictionary<ModeloMoto, string> DisplayNames = new()
	{
		{ ModeloMoto.FACTOR_125_I_UBS_2017_2025, "Factor 125 I UBS (2017-2025)" },
		{ ModeloMoto.FACTOR_150_UBS_2016_2024, "Factor 150 UBS (2016-2024)" },
		{ ModeloMoto.NOVA_FACTOR_2025, "Nova Factor 2025" },
		{ ModeloMoto.NOVA_FACTOR_DX_2025, "Nova Factor DX 2025" },
		{ ModeloMoto.FAZER_150_UBS_2014_2025, "Fazer 150 UBS (2014-2025)" },
		{ ModeloMoto.FZ15_2023_2024, "FZ15 (2023-2024)" },
		{ ModeloMoto.FAZER_FZ15_ABS_CONNECTED_2025_2026, "Fazer FZ15 ABS Connected (2025-2026)" },
		{ ModeloMoto.FAZER_250_2018_2024, "Fazer 250 (2018-2024)" },
		{ ModeloMoto.FZ25_CONNECTED_2025_2026, "FZ25 Connected (2025-2026)" },
		{ ModeloMoto.CROSSER_XTZ150_S_2015_2024, "Crosser XTZ150 S (2015-2024)" },
		{ ModeloMoto.CROSSER_XTZ150_S_2025, "Crosser XTZ150 S 2025" },
		{ ModeloMoto.CROSSER_XTZ150_Z_2015_2024, "Crosser XTZ150 Z (2015-2024)" },
		{ ModeloMoto.CROSSER_XTZ150_Z_2025, "Crosser XTZ150 Z 2025" },
		{ ModeloMoto.LANDER_250_2020_2024, "Lander 250 (2020-2024)" },
		{ ModeloMoto.LANDER_250_ABS_CONNECTED_2025, "Lander 250 ABS Connected 2025" },
		{ ModeloMoto.TENERE_700, "Ténéré 700" },
		{ ModeloMoto.R15_ABS_2024_2026, "R15 ABS (2024-2026)" },
		{ ModeloMoto.YZF_R3_ABS_2020_2025, "YZF-R3 ABS (2020-2025)" },
		{ ModeloMoto.YZF_R3_ABS_CONNECTED_2026, "YZF R3 ABS Connected 2026" },
		{ ModeloMoto.MT03_ABS_2021_2025, "MT03 ABS (2021-2025)" },
		{ ModeloMoto.NOVA_MT03_CONNECTED_2026, "Nova MT-03 Connected 2026" },
		{ ModeloMoto.NOVA_MT07_CONNECTED_2026, "Nova MT-07 Connected 2026" },
		{ ModeloMoto.NEO_125_2017_2024, "Neo 125 (2017-2024)" },
		{ ModeloMoto.NEOS_DUAL_CONNECTED_2026, "Neo's Dual Connected 2026" },
		{ ModeloMoto.FLUO_ABS_2022_2025, "Fluo ABS (2022-2025)" },
		{ ModeloMoto.FLUO_ABS_HYBRID_CONNECTED_2026, "Fluo ABS Hybrid Connected 2026" },
		{ ModeloMoto.AEROX_ABS_CONNECTED, "Aerox ABS Connected" },
		{ ModeloMoto.NMAX_CONNECTED_2023_2024, "NMAX Connected (2023-2024)" },
		{ ModeloMoto.NOVA_NMAX_CONNECTED_2025, "Nova NMAX Connected 2025" },
		{ ModeloMoto.XMAX_2021_2024, "XMAX (2021-2024)" },
		{ ModeloMoto.XMAX_CONNECTED_2025, "XMAX Connected 2025" },
		{ ModeloMoto.NOVA_XMAX_300_CONNECTED_2026, "Nova XMAX 300 Connected 2026" }
	};

	/// <summary>
	/// Retorna o nome legível do modelo
	/// </summary>
	public static string GetDisplayName(this ModeloMoto modelo)
	{
		return DisplayNames.TryGetValue(modelo, out var name) ? name : modelo.ToString();
	}
}
