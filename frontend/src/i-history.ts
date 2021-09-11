export interface IHistory {
    action: string,
    block: any,
    createHref: any,
    go: any,
    goBack: () => void,
    goForward: () => void,
    length: number,
    listen: (listener: any) => void,
    location: { pathname: string, search: string, hash: string, state?: any },
    push: (path: string, state?: any) => void,
    replace: (path: string, state?: any) => void
}