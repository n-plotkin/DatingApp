import { User } from "./user";

export class UserParams {
    typeof = "song";
    minAge = 18;
    maxAge = 99;
    pageNumber = 1;
    pageSize = 5;
    
    orderBy = "lastActive"

    constructor(user: User) {
    }
}