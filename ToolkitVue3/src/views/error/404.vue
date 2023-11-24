<script lang="tsx">
import { defineComponent } from 'vue'
import { Result, Button } from 'ant-design-vue'
import { useRouter } from 'vue-router'

export default defineComponent({
  name: '404',
  setup() {
    const router = useRouter()
    return () => (
      <Result
        status="404"
        title="404"
        sub-title="您访问的页面不存在"
        v-slots={{
          extra: () => (
            <Button type="primary" onClick={() => router.replace('/')}>
              返回待办
            </Button>
          )
        }}
      ></Result>
    )
  }
})
</script>
