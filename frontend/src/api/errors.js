// Tradução dos erros de conversão do JSON (chaves "$.campo"), que o ASP.NET
// devolve em inglês antes de chegar à validação dos DTOs.
const CONVERSION_MESSAGES = {
	budgetitemid: "Selecione o item da despesa.",
	categoryid: "Selecione a categoria do item.",
	unitvalue: "Informe um valor maior que zero.",
	quantity: "Informe uma quantidade de pelo menos 1.",
	date: "Informe a data da despesa."
};

/**
 * Monta a mensagem de erro a partir do corpo de erro da API: usa `message`
 * quando existir; senão, junta só o texto das mensagens de `errors` (resposta
 * de validação 400). Nunca devolve o JSON bruto.
 */
export function apiErrorMessage(error, fallback) {
	if (error?.message) {
		return error.message;
	}

	const errors = error?.errors || {};

	// Quando o JSON não converte (ex.: data ""), a API também acusa o parâmetro
	// inteiro (`dto`), o que só repete o erro do campo: ignora se houver outro.
	let fields = Object.keys(errors);
	if (fields.length > 1) {
		fields = fields.filter(field => field !== "dto");
	}

	const messages = fields.flatMap(field => {
		if (field.startsWith("$.")) {
			const message = CONVERSION_MESSAGES[field.slice(2).toLowerCase()];
			return message ? [message] : [];
		}

		return errors[field];
	});

	return [...new Set(messages)].join(" ") || fallback;
}

/**
 * Para respostas lidas como texto: se o texto for o JSON de erro da API,
 * monta a mensagem pelos campos; se for texto simples, repassa o texto.
 */
export function apiErrorMessageFromText(text, fallback) {
	try {
		const error = JSON.parse(text);

		if (error && typeof error === "object") {
			return apiErrorMessage(error, fallback);
		}
	} catch {
		// não é JSON: usa o texto como está
	}

	return text || fallback;
}
