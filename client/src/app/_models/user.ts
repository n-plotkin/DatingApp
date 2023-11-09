export interface User {
    //a typescript interface doesn't need an i prefix
    username: string;
    token: string;
    photoUrl: string;
    knownAs: string;
    gender: string;
}