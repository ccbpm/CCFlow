<template>
    <div id="IFrameComponent">
        <el-scrollbar>
            <el-card style="height:99%;width:99%">
                <iframe
                        class="el_sence_table"
                        ref="senTable"
                        frameborder="0"
                        height="100%"
                        name="senceTable"
                        scrollong="no"
                        width="100%"
                        :src="src">
                </iframe>
            </el-card>
        </el-scrollbar>
    </div>
</template>

<script>
    export default {
        name: "IFrameComponent",
        data(){
            return{
                params:{},
                src:""
            }
        },
        created(){
            this.params=this.$slots.getters.getData;
            this.src = this.params.IFrameSrc;
        }
    }
</script>

<style scoped>

</style>