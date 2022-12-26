export interface IUser {
    id: string;
    firstName: string;
    lastName: string;
    userName: string | null;
    avatarUrl: string | null;

    followsMe: boolean;
    followedByMe: boolean;
}
