import { User } from './user';

export class UserParams {
    
    selectedGender: string;
    minAge = 18;
    maxAge = 99;
    pageNumber = 1;
    pageSize = 5;
    orderBy = 'LastActive';

    constructor(user: User) {
        this.selectedGender = user.gender === 'female' ? 'male' : 'female';
    }
}