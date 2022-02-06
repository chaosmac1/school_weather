export default async function (input: RequestInfo, init?: RequestInit): Promise<Response | null> {
    try {
        return await fetch(input, init);
    } catch (e) {
        return null;
    }
}

export async function fetchNullJson<T>(input: RequestInfo, init?: RequestInit): Promise<T | null> {
    try {
        const rep = await fetch(input, init);
        return rep.json();
    } catch (e) {
        return null;
    }
}
