import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MembersList } from '../features/members/members-list/members-list';
import { MembersDetailed } from '../features/members/members-detailed/members-detailed';
import { Lists } from '../features/lists/lists';
import { Messages } from '../features/messages/messages';
import { authGuard } from '../core/guards/auth-guard';
import { TestErrors } from '../features/test-errors/test-errors';
import { NotFound } from '../shared/errors/not-found/not-found';
import { ServerError } from '../shared/errors/server-error/server-error';
import { MemberProfile } from '../features/members/member-profile/member-profile';
import { MemberMessages } from '../features/members/member-messages/member-messages';
import { memberResolver } from '../features/members/member-resolver';
import { MemberPhotos } from '../features/members/member-photos/member-photos';
import { preventUnsavedChangesGuard } from '../core/guards/prevent-unsaved-changes-guard';
import { Admin } from '../features/admin/admin';
import { adminGuard } from '../core/guards/admin-guard';

export const routes: Routes = [
    {
        path: '',
        component: Home
    },

    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [{
            path: 'member/:id',
            resolve:{member:memberResolver}, // when we load up the member-detailed component we resolve the member..so we go and get the member..
            runGuardsAndResolvers: 'always',
            component: MembersDetailed,
            children:[
                {path:'', redirectTo:'profile', pathMatch: 'full'},
                {path:'profile', component:MemberProfile, title:'Profile', canDeactivate:[preventUnsavedChangesGuard]},
                {path:'photos', component:MemberPhotos, title:'Photos'},
                {path:'messages', component:MemberMessages, title:'Messages'}
            ]
        },
        {
            path: 'lists',
            component: Lists
        },
        {
            path: 'messages',
            component: Messages
        },
        {
            path: 'members',
            component: MembersList,
        },
        ]
    },

    {
        path:'errors',
        component:TestErrors
    },

    {
        path:'server-error',
        component:ServerError
    },
    {
        path:'admin',
        component:Admin,
        canActivate:[adminGuard]
    },

    // root that doesnt exist
    {
        path: '**',
        component: NotFound
    }
];
