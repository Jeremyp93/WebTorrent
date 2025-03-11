export function roundDecimal (number: number, decimals: number = 1) {
    const factor = Math.pow(10, decimals);
    return Math.round(number * factor) / factor;
}