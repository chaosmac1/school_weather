export const TicksPerMillisecond: number = 10000;
export const TicksPerSecond: number = TicksPerMillisecond * 1000;
export const TicksPerMinute: number = TicksPerSecond * 60;
export const TicksPerHour: number = TicksPerMinute * 60;
export const TicksPerDay: number = TicksPerHour * 24;

export function dateToTicks(time: Date): number {
    return ((time.getTime() * 10000) + 621355968000000000);
}