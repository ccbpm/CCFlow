import { defineStore } from "pinia";
interface projectSetting {
    headerSetting : headerSetting,
    siderSetting : siderSetting,
}
interface headerSetting {
    isShow: boolean,
}
interface siderSetting {
    isShow: boolean,
}
export const useProjectStore = defineStore({
    id: 'project-setting',
    state:(): { projectSetting: projectSetting } => ({
        projectSetting: {
            headerSetting: {
                isShow: true,
            },
            siderSetting: {
                isShow: true,
            }
        }
    }),
    getters: {
        getHeaderSetting() : headerSetting {
            return this.projectSetting.headerSetting;
        },
        getSiderSetting() : siderSetting {
            return this.projectSetting.siderSetting;
        }
    },
    actions: {
        setHeaderShow( show: boolean ){
            this.projectSetting.headerSetting.isShow = show;
        },
        setSiderShow( show: boolean ){
            this.projectSetting.siderSetting.isShow = show;
        },
    }
    
});