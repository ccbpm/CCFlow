// 实体
import request from '@/utils/request'
import { createWhereArgs } from '@/utils/gener/ParamsUtils'
import { REQUEST_URL } from '@/config/EnvProperties'

export default class Entities {
  private readonly EnsName: string = ''
  private queryArgs: Array<string> = []
  private data: Array<any> = []
  // 初始化实体类
  constructor(EnsName: string, ...args: Array<string>) {
    this.EnsName = EnsName
    this.queryArgs = args
  }

  // 设置查询条件
  public async Retrieve(...args: Array<string | any>) {
    if (args.length % 2 === 1) {
      throw new Error('查询条件不匹配')
    }
    this.queryArgs = args
    await this.Init()
  }

  public async Init() {
    this.data = await request.post<null, Array<any>>(REQUEST_URL, null, {
      params: {
        DoType: 'Entities_Init',
        EnsName: this.EnsName,
        Paras: createWhereArgs(this.queryArgs)
      }
    })
  }

  // 是否为空数据
  public isEmpty() {
    return this.data.length === 0
  }

  public getData() {
    return this.data
  }
}
