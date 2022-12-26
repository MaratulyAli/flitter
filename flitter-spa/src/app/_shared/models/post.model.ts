import { IPoll, IPollCreate } from "./poll.model";
import { IUser } from "./user.model";

export interface IPost {
    id: number;
    text: string;
    originalPostId: number | null;
    userId: string;
    likesCount: number;
    liked: boolean;
    poll: IPoll | null;
    user: IUser;
    images: IPostImage[] | null;
    createdAt: string;
}

export interface IPostImage {
    id: number;
    url: string
}

export interface IPostCreate {
    text: string;
    poll: IPollCreate | null;
}

export interface IPostUpdate {
    id: number;
    text: string;
    poll: IPollCreate | null;
}
